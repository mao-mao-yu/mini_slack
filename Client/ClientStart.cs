using System;
using Client;
using System.Threading;
using System.Threading.Tasks;

internal class ClientStart
{
    private static void Main(string[] args)
    {
        AppClient client = new AppClient();
        client.Start("192.168.10.111",8888);
    }
}

