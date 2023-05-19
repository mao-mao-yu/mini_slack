using System;
using Client;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using Client.ClientCommunication;
using System.Collections.Generic;
using Newtonsoft.Json;

internal class ClientStart
{
    private static async Task Main(string[] args)
    {
        UClient udpClient = new UClient("192.168.0.115", 11002, 11000);
        Dictionary<string, string> test = new Dictionary<string, string>() 
        {
            {"action","login"},
            {"username","1228315965"},
            {"password","XfkldptY4327" }
        };
        string jsonStr = JsonConvert.SerializeObject(test);
        while (true)
        {
            await udpClient.Send(jsonStr);
            Thread.Sleep(1000);
        }
        

        
        //AppClient client = new AppClient();
        //client.Start("192.168.10.111",8888);
    }
}

