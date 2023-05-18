using System;
using Server.Data;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using Server;

internal class ServerStart
{
    private static void Main(string[] args)
    {
        AppServer server = new AppServer();
        server.Start();
    }

}