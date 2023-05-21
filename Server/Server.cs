using System;
using System.Collections.Generic;
using System.Text;
using Server.IServer;
using System.Net;
using Server.SocketAsyncCore;

namespace Server
{
    public class AppServer : SocketAsyncTcpServer
    {
        public AppServer(int listenPort, int maxClient) : base(IPAddress.Any, listenPort, maxClient)
        {
        }

        public AppServer(IPAddress localIPAddress, int listenPort, int maxClient) : base(localIPAddress, listenPort, maxClient)
        {
        }

        public AppServer(IPEndPoint localEP, int maxClient) : base(localEP, maxClient)
        {
        }

        protected override void ActionHandler(string data)
        {
            throw new NotImplementedException();
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
