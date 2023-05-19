using System;
using System.Collections.Generic;
using System.Text;
using Server.IServer;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Data;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;
using Server.ServerCommunication;

namespace Server
{
    public class AppServer : IAppServer
    {
        private readonly MyServer server;
        public AppServer(int udpPort, int tcpPort)
        {
            // server
            server = new MyServer(udpPort, tcpPort);
        }

        public void Start()
        {
            server.TcpStartListening();
            server.UdpStartReceiving();
        }

        public bool CheckPassword(string password)
        {
            return true;
        }

        public bool CheckUsername(string username)
        {

            return true;
        }
        public void FileHandler()
        {
            throw new NotImplementedException();
        }

        public void Listening()
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}
