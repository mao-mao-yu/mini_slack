using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Common.Encryption;
using Common.Log;
using Common;
using Server.Models;
using Server.Error;
using Server.SerSetting;
using System.IO;

namespace Server.SocketAsyncCore
{
    public abstract class SocketAsyncTcpServer : IDisposable
    {

        const int opsToPreAlloc = 2;

        #region Fields
        /// <summary>
        /// Max client num
        /// </summary>
        private readonly int _maxClient;

        /// <summary>
        /// Int size
        /// </summary>
        private const int INT_SIZE = sizeof(int);

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
        private readonly int _bufferSize;

        /// <summary>
        /// Ring buffer size
        /// </summary>
        private readonly int _ringBuffersize;

        /// <summary>
        /// Dead thread
        /// </summary>
        private SocketAsyncDaemonThread m_daemonThread;

        /// <summary>
        /// Aes key size
        /// </summary>
        private readonly int _aesKeySize;

        /// <summary>
        /// 信号
        /// </summary>
        Semaphore _maxAcceptedClients;

        /// <summary>
        /// Buffer manager
        /// </summary>
        SocketAsyncBufferManager _bufferManager;

        /// <summary>
        /// Object pool
        /// </summary>
        SocketAsyncEventArgsPool _socketAsyncEventArgsPool;

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
        public Encoding DefaultEncoding { get; set; }
        #endregion

        #region ctor
        /// <summary>  
        /// Async Socket TCP server 
        /// </summary>  
        /// <param name="localIPAddress">Listening IPAddress</param>  
        /// <param name="listenPort">Listening port</param>  
        /// <param name="maxClient">Max client</param>  
        public SocketAsyncTcpServer()
        {
            try
            {
                Setting setting = Common.Setting.SettingBase.LoadSetting<Setting>(Const.SERVER_SETTING_PATH);
                Address = IPAddress.Parse(setting.ServerIP);
                Port = setting.Port;
                DefaultEncoding = Encoding.GetEncoding(setting.DefaultEncoding);
                _maxClient = setting.MaxClientNum;
                _bufferSize = setting.BufferManagerSize;
                _ringBuffersize = setting.RingBufferSize;
                _aesKeySize = setting.AesKeySize;
            }
            catch (ArgumentException e)
            {
                Logger.FERROR(e, "Server setting data is incorrect...");
            }
            catch (FileNotFoundException e)
            {
                Logger.FERROR(e, "Server setting file not found...");
            }
            catch (Exception e)
            {
                Logger.FERROR(e, "Unknown error");
            }

            _socketServer = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _bufferManager = new SocketAsyncBufferManager(_bufferSize * (_maxClient + 1) * opsToPreAlloc, _bufferSize);

            _socketAsyncEventArgsPool = new SocketAsyncEventArgsPool(_maxClient);

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
                Logger.ALL("Set readWriteEventArg buffer...");

                // add SocketAsyncEventArg to the pool  
                _socketAsyncEventArgsPool.Push(readWriteEventArg);
                Logger.ALL("Push a args to SocketAsyncEventArgsPool...");
            }
            Logger.DEBUG("Inited SocketAsyncEventArgs...");
        }

        private AsyncUserToken InitUserToken(SocketAsyncEventArgs asyniar, Socket s)
        {
            AsyncUserToken token = (AsyncUserToken)asyniar.UserToken;
            // Socket
            token.Socket = s;
            // GUID
            token.GUID = Guid.NewGuid().ToString();
            // Connect time
            token.ConnectDateTime = DateTime.Now;
            // Set ring buffer
            token.SetRingBuffer(_ringBuffersize);
            // Generate random rsa key
            (token.RsaPublicKey, token.RsaPrivateKey) = RsaEncryptor.GenerateKeys();
            // Generate random aes key
            token.AesKey = AesEncrypter.GenerateRandomKey(_aesKeySize);
            token.AesIV = AesEncrypter.GenerateRandomIV();

            return token;
        }

        private byte[] InitFirstPacket(AsyncUserToken token)
        {
            byte[] rsaPublicKey = DefaultEncoding.GetBytes(token.RsaPublicKey);
            byte[] GUID = DefaultEncoding.GetBytes(token.GUID);
            byte[] packet = new byte[rsaPublicKey.Length + GUID.Length];
            rsaPublicKey.CopyTo(packet, 0);
            GUID.CopyTo(packet, rsaPublicKey.Length);

            Logger.DEBUG($"publc key: {rsaPublicKey.Length} bytes GUID: {GUID.Length} bytes");
            return packet;
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
                Logger.INFO($"Tcp server start listening on {Address}:{Port}");
                _socketServer.Listen(_maxClient);

                // Start accept
                StartAccept(null);

                // Check daemon thread.
                m_daemonThread = new SocketAsyncDaemonThread(this);
                //m_daemonThread.DaemonThreadStart();
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
                Logger.FERROR(E, msg);
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
                        SocketAsyncEventArgs asyniar = _socketAsyncEventArgsPool.Pop();
                        //用户的token操作
                        AsyncUserToken token = InitUserToken(asyniar, s);
                        SocketUserTokenList.Add(asyniar);               //Add event to token list
                        byte[] rsaPacket = InitFirstPacket(token);

                        Send(s, rsaPacket, 0, rsaPacket.Length, 1000);

                        Logger.FINFO($"Client {s.RemoteEndPoint} connected, Have {_clientCount} clients.");

                        if (!s.ReceiveAsync(asyniar))       // ProcessReceive
                        {
                            ProcessReceive(asyniar);
                        }
                    }
                    catch (SocketException ex)
                    {
                        Logger.FERROR(ex, $"Receive user {s.RemoteEndPoint} error, message: {ex.Message}");
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
                Socket s = e.AcceptSocket; // Connecting to client's socket
                if (s.Connected)
                {
                    // create new byte array including length header
                    byte[] dataWithLengthHeader = new byte[INT_SIZE + data.Length];

                    // convert length to bytes
                    byte[] lengthHeader = BitConverter.GetBytes(data.Length);
                    lengthHeader.CopyTo(dataWithLengthHeader, 0);

                    // copy data into new array
                    data.CopyTo(dataWithLengthHeader, INT_SIZE);

                    Array.Copy(dataWithLengthHeader, 0, e.Buffer, 0, dataWithLengthHeader.Length);

                    // Submit the send request, this function may send it synchronously,
                    if (!s.SendAsync(e))
                    {
                        // When sending synchronously, handle the send completion event.
                        Logger.DEBUG("Sended " + dataWithLengthHeader.Length);
                        ProcessSend(e);
                    }
                }
            }
        }

        /// <summary>
        /// Local send
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        private int Send(Socket socket, byte[] data)
        {
            byte[] header = BitConverter.GetBytes(data.Length);
            byte[] packet = new byte[data.Length + INT_SIZE];
            header.CopyTo(packet, 0);

            data.CopyTo(packet, INT_SIZE);
            return socket.Send(packet);
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

            // Create new buffer with 4 bytes length header
            byte[] bufferWithLengthHeader = new byte[INT_SIZE + buffer.Length];

            // Convert length to bytes and add to the new buffer
            byte[] lengthHeader = BitConverter.GetBytes(buffer.Length);
            Array.Copy(lengthHeader, 0, bufferWithLengthHeader, 0, INT_SIZE);

            // Copy the actual data into new buffer
            Array.Copy(buffer, 0, bufferWithLengthHeader, INT_SIZE, buffer.Length);

            int sent = 0;  // how many bytes is already sent
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                {
                    throw new TimeoutException("Timeout.");
                }
                try
                {
                    sent += socket.Send(bufferWithLengthHeader, offset + sent, size - sent + INT_SIZE, SocketFlags.None);
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
                        throw ex;  // any serious error occur
                    }
                }
            } while (sent < size + INT_SIZE);
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
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                token.ActiveDateTime = DateTime.Now;
                Socket socket = token.Socket;
                SocketAsyncRingBuffer ringBuffer = token.Rb;
                if (socket == null || !socket.Connected)
                {
                    return;
                }

                // Copy the received data to the ring buffer
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);

                // 如果环形数组为空或者有空间则写入数据
                if (ringBuffer.IsEmpty || ringBuffer.HavingSpace(e.BytesTransferred))
                {
                    Logger.DEBUG($"479: Written {data.Length} to ring buffer...");
                    ringBuffer.Write(data);
                }

                try
                {
                    while (true)
                    {
                        if (!ringBuffer.HavingData(INT_SIZE))
                        {
                            Logger.DEBUG($"Haven't int data...");
                            break;
                        }
                        int packetLength = ringBuffer.ReadHead() + INT_SIZE;
                        int dataLength = packetLength - INT_SIZE;
                        if (!ringBuffer.HavingData(packetLength))
                        {
                            Logger.DEBUG($"Haven't {packetLength} bytes data...");
                            break;
                        }
                        else
                        {
                            byte[] packet = ringBuffer.Read(packetLength);
                            byte[] bytesData = new byte[dataLength];
                            Array.Copy(packet, INT_SIZE, bytesData, 0, dataLength);
                            Logger.INFO($"Received {DefaultEncoding.GetString(bytesData)} from {((AsyncUserToken)e.UserToken).Socket.RemoteEndPoint}");
                        }
                    }
                }
                catch (BytesDataHeaderError ex)
                {
                    Logger.FERROR(ex, "Received error data");
                    ringBuffer.Clear();
                }
                catch (Exception ex)
                {
                    Logger.FERROR(ex, "Unknown error");
                }

                // Asynchronously receive more data from the socket
                if (!socket.ReceiveAsync(e))
                {
                    Thread.Sleep(100);
                    // If ReceiveAsync completed synchronously, call ProcessReceive again to process the received data
                    ProcessReceive(e);
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
                Logger.FERROR(E, msg);
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
            if (!(e.UserToken is AsyncUserToken token))
            {
                e.AcceptSocket.Close();
                _maxAcceptedClients.Release();//释放线程信号量
                return;
            }

            if (e.SocketError == SocketError.OperationAborted || e.SocketError == SocketError.ConnectionAborted)
                return;
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
                Logger.DEBUG($"Client {s.RemoteEndPoint} disconnected in {DateTime.Now:yyyy-MM-dd HH:mm:ss ffff}.");
                Logger.FDEBUG($"Client {s.RemoteEndPoint} disconnected in {DateTime.Now:yyyy-MM-dd HH:mm:ss ffff}.");
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
            _socketAsyncEventArgsPool.Push(e);            //SocketAsyncEventArg 对象被释放，压入可重用队列。
            SocketUserTokenList.Remove(e);  //去除正在连接的用户
            Logger.DEBUG($"Now have {SocketUserTokenList.Count()} clients");
            Logger.FDEBUG($"Now have {SocketUserTokenList.Count()} clients");
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

        protected abstract void HandleMessage(string data);
    }
}
