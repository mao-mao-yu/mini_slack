using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Server.Encryption;
using Server.Log;

namespace Server.SocketCore
{
    public abstract class SocketAsyncTcpServer : IDisposable
    {

        const int opsToPreAlloc = 2;

        #region Fields
        /// <summary>
        /// Max client num
        /// </summary>
        private int _maxClient;

        /// <summary>
        /// Logger
        /// </summary>
        protected Logger lg;

        /// <summary>
        /// Listener Socket
        /// </summary>
        private Socket _socketServer;

        /// <summary>
        /// The num of online client
        /// </summary>
        private int _clientCount;

        /// <summary>
        /// Buffer size
        /// </summary>
        private int _bufferSize = 2048;

        /// <summary>
        /// Dead thread
        /// </summary>
        private DaemonThread m_daemonThread;

        /// <summary>
        /// 信号
        /// </summary>
        Semaphore _maxAcceptedClients;

        /// <summary>
        /// Buffer manager
        /// </summary>
        BufferManager _bufferManager;

        /// <summary>
        /// Object pool
        /// </summary>
        SocketAsyncEventArgsPool _objectPool;

        /// <summary>
        /// Online user
        /// </summary>
        public SocketAsyncEventArgsList SocketUserTokenList;

        private int m_socketTimeOutMS = 1; // ms

        public int SocketTimeOutMS
        {
            get
            {
                return m_socketTimeOutMS;
            }
            set
            {
                m_socketTimeOutMS = value;
            }
        }

        private readonly object AcceptLocker = new object();

        private bool disposed = false;
        #endregion

        #region Properties
        /// <summary>  
        /// Server is running
        /// </summary>  
        public bool IsRunning { get; private set; }

        /// <summary>  
        /// Listening addr
        /// </summary>  
        public IPAddress Address { get; private set; }

        /// <summary>  
        /// Listening port
        /// </summary>  
        public int Port { get; private set; }

        /// <summary>  
        /// Encoding format   
        /// </summary>  
        public Encoding Encoding { get; set; }
        #endregion

        #region ctor
        /// <summary>  
        /// Async IOCP SOCKET server 
        /// </summary>  
        /// <param name="listenPort">listening port</param>  
        /// <param name="maxClient">Max client</param>  
        public SocketAsyncTcpServer(int listenPort, int maxClient)
            : this(IPAddress.Any, listenPort, maxClient)
        {
        }

        /// <summary>  
        /// Async Socket TCP server
        /// </summary>  
        /// <param name="localEP">Listening end point</param>  
        /// <param name="maxClient">Max client</param>  
        public SocketAsyncTcpServer(IPEndPoint localEP, int maxClient)
            : this(localEP.Address, localEP.Port, maxClient)
        {
        }

        /// <summary>  
        /// Async Socket TCP server 
        /// </summary>  
        /// <param name="localIPAddress">Listening IPAddress</param>  
        /// <param name="listenPort">Listening port</param>  
        /// <param name="maxClient">Max client</param>  
        public SocketAsyncTcpServer(IPAddress localIPAddress, int listenPort, int maxClient)
        {
            Address = localIPAddress;
            Port = listenPort;
            Encoding = Encoding.Default;

            _maxClient = maxClient;

            _socketServer = new Socket(localIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _bufferManager = new BufferManager(_bufferSize * (_maxClient + 1) * opsToPreAlloc, _bufferSize);

            _objectPool = new SocketAsyncEventArgsPool(_maxClient);

            SocketUserTokenList = new SocketAsyncEventArgsList();

            _maxAcceptedClients = new Semaphore(_maxClient, _maxClient);
        }

        #endregion

        #region Init

        /// <summary>  
        /// 初始化函数  
        /// </summary>  
        public void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds   
            // against memory fragmentation  
            _bufferManager.InitBuffer();
            lg = new Logger();
            // preallocate pool of SocketAsyncEventArgs objects  
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < _maxClient; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs  
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                readWriteEventArg.UserToken = new AsyncUserToken();

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object  
                _bufferManager.SetBuffer(readWriteEventArg);

                // add SocketAsyncEventArg to the pool  
                _objectPool.Push(readWriteEventArg);
            }

        }

        #endregion

        #region Start
        /// <summary>  
        /// 启动  
        /// </summary>  
        public void Start()
        {
            if (!IsRunning)
            {
                Init();
                IsRunning = true;
                IPEndPoint localEndPoint = new IPEndPoint(Address, Port);
                // 创建监听socket  
                _socketServer = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socketServer.ReceiveBufferSize = _bufferSize;
                _socketServer.SendBufferSize = _bufferSize;
                if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // 配置监听socket为 dual-mode (IPv4 & IPv6)   
                    // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,  
                    _socketServer.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                    _socketServer.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
                }
                else
                {
                    _socketServer.Bind(localEndPoint);
                }
                // Start listening
                lg.INFO($"Tcp server start listening on {Address}:{Port}");
                _socketServer.Listen(_maxClient);

                // Start accept
                StartAccept(null);

                // Check daemon thread.
                m_daemonThread = new DaemonThread(this);
                //m_daemonThread.DaemonThreadStart();
            }
        }
        #endregion

        #region Stop

        /// <summary>  
        /// Stop server  
        /// </summary>  
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _socketServer.Close();
                //Close all socket 
                SocketUserTokenList.CloseAll();
            }
        }

        #endregion

        #region Accept
        /// <summary>  
        /// Start accept  
        /// </summary>  
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                //socket must be cleared since the context object is being reused  
                acceptEventArg.AcceptSocket = null;
            }
            _maxAcceptedClients.WaitOne();

            if (!_socketServer.AcceptAsync(acceptEventArg))
            {
                //如果I/O挂起等待异步则触发AcceptAsyn_Asyn_Completed事件  
                //此时I/O操作同步完成，不会触发Asyn_Completed事件，所以指定BeginAccept()方法  
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>  
        /// Call back func 
        /// </summary>  
        /// <param name="sender">Object who raised the event.</param>  
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>  
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                lock (AcceptLocker)
                {
                    ProcessAccept(e);
                }
            }
            catch (Exception E)
            {
                string msg = $"Accept client {e.AcceptSocket} error, message: {E.Message}";
                lg.IMPORTTANT(E, msg);
            }
        }

        /// <summary>  
        /// Process a accept
        /// </summary>  
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>  
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Socket s = e.AcceptSocket;//和客户端关联的socket  
                if (s != null && s.Connected)
                {
                    try
                    {

                        Interlocked.Increment(ref _clientCount);        //原子操作加1  
                        SocketAsyncEventArgs asyniar = _objectPool.Pop();
                        AsyncUserToken token = (AsyncUserToken)asyniar.UserToken;

                        //用户的token操作
                        token.Socket = s;
                        token.ID = Guid.NewGuid().ToString();
                        token.ConnectDateTime = DateTime.Now;
                        (string publicKey, string privateKey) = RsaEncryptor.GenerateKeys();
                        token.SetRsaKeys(publicKey, privateKey);

                        SocketUserTokenList.Add(asyniar);   //Add event to token list

                        // If conncet successful. Send GUID and RSA Keys
                        //s.Send(Encoding.UTF8.GetBytes(token.ID));
                        s.Send(Encoding.UTF8.GetBytes(token.ID));
                        lg.FINFO($"Client {s.RemoteEndPoint} connected, Have {_clientCount} clients.");

                        if (!s.ReceiveAsync(asyniar))       // ProcessReceive
                        {
                            ProcessReceive(asyniar);
                            //ProcessReceive(asyniar);
                        }
                    }
                    catch (SocketException ex)
                    {
                        lg.IMPORTTANT(ex, $"Receive user {s.RemoteEndPoint} error, message: {ex.Message}");
                    }
                    //投递下一个接受请求  
                    StartAccept(e);
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }
        #endregion

        #region Send message

        /// <summary>  
        /// Async send data 
        /// </summary>  
        /// <param name="e">SocketAsyncEventArgs</param>  
        /// <param name="data">Bytes data</param>  
        public void Send(SocketAsyncEventArgs e, byte[] data)
        {
            if (e.SocketError == SocketError.Success)
            {
                Socket s = e.AcceptSocket;                              // Connecting to client's socket
                if (s.Connected)
                {
                    Array.Copy(data, 0, e.Buffer, 0, data.Length);      // Set the data to be sent 

                    //e.SetBuffer(data, 0, data.Length);                // Set the data to be sent
                    //
                    // Submit the send request, this function may send it synchronously,
                    // In which case it returns false and does not trigger the SocketAsyncEventArgs.Completed event.
                    if (!s.SendAsync(e))
                    {
                        // When sending synchronously, handle the send completion event.
                        lg.DEBUG("Sended " + data.Length);
                        ProcessSend(e);
                    }
                    else
                    {
                        //CloseClientSocket(e);
                    }
                }
            }
        }

        /// <summary>  
        /// 同步的使用socket发送数据  
        /// </summary>  
        /// <param name="socket"></param>  
        /// <param name="buffer"></param>  
        /// <param name="offset"></param>  
        /// <param name="size"></param>  
        /// <param name="timeout"></param>  
        public void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            socket.SendTimeout = 0;
            int startTickCount = Environment.TickCount;
            int sent = 0; // how many bytes is already sent  
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                {
                    throw new TimeoutException("Timeout.");
                }
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                    ex.SocketErrorCode == SocketError.IOPending ||
                    ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again  
                        Thread.Sleep(30);
                    }
                    else
                    {
                        throw ex; // any serious error occurr  
                    }
                }
            } while (sent < size);
        }


        /// <summary>  
        /// 发送完成时处理函数  
        /// </summary>  
        /// <param name="e">与发送完成操作相关联的SocketAsyncEventArg对象</param>  
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                Socket s = (Socket)token.Socket;

                //TODO  
                token.ActiveDateTime = DateTime.Now;
            }
            else
            {
                CloseClientSocket(e);
            }
        }
        #endregion

        #region Recv message
        /// <summary>  
        ///接收完成时处理函数  
        /// </summary>  
        /// <param name="e">与接收完成操作相关联的SocketAsyncEventArg对象</param>  
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)//if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) 
            {
                // 检查远程主机是否关闭连接  
                if (e.BytesTransferred > 0)
                {
                    AsyncUserToken token = (AsyncUserToken)e.UserToken;
                    token.ActiveDateTime = DateTime.Now;
                    Socket s = token.Socket;
                    if (s == null || !s.Connected)
                    {
                        return;
                    }
                    //判断所有需接收的数据是否已经完成  
                    if (s.Available == 0)
                    {
                        byte[] data = new byte[e.BytesTransferred];
                        lg.DEBUG($"Data length:{data.Length}");
                        Array.Copy(e.Buffer, e.Offset, data, 0, data.Length);
                        int intLength = sizeof(int);
                        int offset = 0;
                        while (offset < e.BytesTransferred)
                        {
                            byte[] dataLengthByte = new byte[intLength];
                            Array.Copy(data, offset, dataLengthByte, 0, intLength);
                            int dataLength = BitConverter.ToInt32(dataLengthByte);
                            lg.DEBUG($"Ont clock data length:{dataLength}");
                            offset += intLength;
                            lg.DEBUG($"Offset index:{offset}");
                            byte[] bytesData = new byte[dataLength];
                            Array.Copy(data, offset, bytesData, 0, dataLength);
                            lg.DEBUG($"收到 {s.RemoteEndPoint} 数据为 {Encoding.UTF8.GetString(bytesData)}");
                            offset += dataLength;
                        }
                        //HandleMessage(data);
                    }
                    if (!s.ReceiveAsync(e))//为接收下一段数据，投递接收请求，这个函数有可能同步完成，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件  
                    {
                        //同步接收时处理接收完成事件  
                        ProcessReceive(e);
                    }
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        #endregion

        #region Callback
        /// <summary>  
        /// 当Socket上的发送或接收请求被完成时，调用此函数  
        /// </summary>  
        /// <param name="sender">激发事件的对象</param>  
        /// <param name="e">与发送或接收完成操作相关联的SocketAsyncEventArg对象</param>  
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = (AsyncUserToken)e.UserToken;
            userToken.ActiveDateTime = DateTime.Now;
            // Determine which type of operation just completed and call the associated handler. 
            try
            {
                lock (userToken)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Accept:
                            ProcessAccept(e);
                            break;
                        case SocketAsyncOperation.Receive:
                            ProcessReceive(e);
                            break;
                        default:
                            throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                    }
                }
            }
            catch (Exception E)
            {
                string msg = $"IO_Completed {userToken.Socket} error, message: {E.Message}";
                lg.IMPORTTANT(E, msg);
            }
        }
        #endregion

        #region Close
        /// <summary>  
        /// 关闭socket连接  
        /// </summary>  
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>  
        public void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;
            if (token == null)
            {
                e.AcceptSocket.Close();
                _maxAcceptedClients.Release();//释放线程信号量
                return;
            }

            if (e.SocketError == SocketError.OperationAborted || e.SocketError == SocketError.ConnectionAborted)
                return;

            lg.DEBUG($"Client {token.Socket.RemoteEndPoint} disconnected in {DateTime.Now:yyyy-MM-dd HH:mm:ss ffff}");
            lg.FDEBUG($"Client {token.Socket.RemoteEndPoint} disconnected");

            Socket s = token.Socket;
            CloseClientSocket(s, e);
        }

        /// <summary>  
        /// 关闭socket连接  
        /// </summary>  
        /// <param name="s"></param>  
        /// <param name="e"></param>  
        private void CloseClientSocket(Socket s, SocketAsyncEventArgs e)
        {
            try
            {
                s.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                throw;
            }
            finally
            {
                s.Close();
            }
            Interlocked.Decrement(ref _clientCount);
            _maxAcceptedClients.Release();  //释放线程信号量
            _objectPool.Push(e);            //SocketAsyncEventArg 对象被释放，压入可重用队列。
            SocketUserTokenList.Remove(e);  //去除正在连接的用户
        }
        #endregion

        #region Dispose
        /// <summary>  
        /// Performs application-defined tasks associated with freeing,   
        /// releasing, or resetting unmanaged resources.  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>  
        /// Releases unmanaged and - optionally - managed resources  
        /// </summary>  
        /// <param name="disposing"><c>true</c> to release   
        /// both managed and unmanaged resources; <c>false</c>   
        /// to release only unmanaged resources.</param>  
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_socketServer != null)
                        {
                            Stop();
                            _socketServer = null;
                        }
                    }
                    catch (SocketException ex)
                    {
                        //TODO 事件  
                        Stop();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
                disposed = true;
            }
        }
        #endregion

        protected abstract void HandleMessage(string data);
    }
}
