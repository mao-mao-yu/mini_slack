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
using Server.SocketAsyncCore;

internal class ServerStart
{
    private static void Main(string[] args)
    {
        // udp,tcp
        //AppServer server = new AppServer(11000, 8888);
        //server.Start();
        //while (true) ;
        //Logger lg = new Logger();
        //for (int i = 0; i < 10; i++)
        //{
        //    lg.FDEBUG(i.ToString());
        //}

        try
        {
            //IPAddress IP = IPAddress.Parse("192.168.0.248");
            IPAddress IP = IPAddress.Parse("192.168.10.111");
            int parallelNum = 5000;
            int port = 8888;
            
            AppServer server = new AppServer(port, parallelNum);
            server.Start();
            Console.WriteLine("Server is started...");
            Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

}