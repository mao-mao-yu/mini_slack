using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Server.Protocol
{
    public class Server
    {
        public class UdpPacket
        {
            public int Length { get; set; }
            public byte[] Data { get; set; }
        }
    }
}
