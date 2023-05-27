using System;
using System.Collections.Generic;
using Client.Setting;
using Client.Data;
using System.Net;
using System.Net.Sockets;
using Client.Log;
using System.Threading;
using System.Text;
using System.IO;

namespace Client.ClientCore
{
    public class AsyncClient
    {
        private ClientSetting _clientSetting;

        private readonly IPAddress serverIP;

        private readonly int port;

        private readonly int bufferSize;

        private readonly RingBuffer ringBuffer;

        private string rsaPublicKey;

        private string GUID;

        private NetworkStream stream;

        private TcpClient client;
        public AsyncClient()
        {
            _clientSetting = SettingBase.LoadSetting<ClientSetting>(Const.CLIENT_SETTING_PATH);
            serverIP = IPAddress.Parse(_clientSetting.ServerIP);
            port = _clientSetting.Port;
            bufferSize = _clientSetting.BufferSize;
            ringBuffer = new RingBuffer(bufferSize);
        }

        public void StartConnect()
        {
            client = new TcpClient();
            client.BeginConnect(serverIP, port, ConnectedCallback, null);
        }

        private void ConnectedCallback(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    client.EndConnect(result);
                    if (!client.Connected)
                    {
                        Logger.FDEBUG($"Server is offline");
                        return;
                    }

                    stream = client.GetStream();
                    try
                    {
                        FirstPacket();
                        StartRecv();
                    }
                    catch (IOException e)
                    {
                        // handle exception when reading
                    }
                }
                catch (Exception e)
                {
                    // log and reconnect
                }
            }
            else
            {
                // log and reconnect
            }
        }

        private void ReconnectAfterDelay()
        {
            Thread.Sleep(5000);
            StartConnect();
        }

        private void FirstPacket()
        {
            stream.Read(new byte[Const.INT_SIZE], 0, Const.INT_SIZE);
            byte[] rsaBytesPublicKey = new byte[Const.RSA_PUBLICKEY_SIZE];
            stream.Read(rsaBytesPublicKey, 0, Const.RSA_PUBLICKEY_SIZE);
            rsaPublicKey = Encoding.UTF8.GetString(rsaBytesPublicKey);

            byte[] bytesGUID = new byte[Const.GUID_SIZE];
            stream.Read(bytesGUID, 0, Const.GUID_SIZE);
            GUID = Encoding.UTF8.GetString(bytesGUID);

            Logger.DEBUG($"Received first packet{rsaPublicKey}\n{GUID}");
        }

        private void StartRecv()
        {
            byte[] buffer = new byte[bufferSize];
            stream.BeginRead(buffer, 0, buffer.Length, ReceivedCallback, buffer);
        }

        private void ReceivedCallback(IAsyncResult result)
        {
            byte[] buffer = (byte[])result.AsyncState;
            int bytesRead = stream.EndRead(result);
            Logger.DEBUG($"Received {bytesRead} bytes data...");
            if (bytesRead > 0)
            {
                if (ringBuffer.HavingSpace(bytesRead))
                {
                    ringBuffer.Write(buffer, 0, bytesRead);
                }
                else
                {
                    ProcessRecv();
                }
            }

            // 继续异步读取操作
            stream.BeginRead(buffer, 0, buffer.Length, ReceivedCallback, buffer);
        }

        private void ProcessRecv()
        {
            while (true)
            {
                if (!ringBuffer.HavingData(Const.INT_SIZE))
                {
                    Logger.DEBUG($"Haven't int data...");
                    break;
                }
                int packetLength = ringBuffer.ReadHead() + Const.INT_SIZE;
                int dataLength = packetLength - Const.INT_SIZE;
                if (!ringBuffer.HavingData(packetLength))
                {
                    Logger.DEBUG($"Haven't {packetLength} bytes data...");
                    break;
                }
                else
                {
                    byte[] packet = ringBuffer.Read(packetLength);
                    byte[] bytesData = new byte[dataLength];
                    Array.Copy(packet, Const.INT_SIZE, bytesData, 0, dataLength);
                    Logger.INFO($"Received {Encoding.UTF8.GetString(bytesData)} from {serverIP}");
                }
            }

        }
    }
}
