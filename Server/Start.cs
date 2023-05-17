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

class Start
{
    static async Task Main(string[] args)
    {
        AppServer server = new AppServer();
        await server.StartListening();
    }

}