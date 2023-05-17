using System;
using Client;
using System.Threading;
using System.Threading.Tasks;

class Start
{
    static async Task Main(string[] args)
    {
        Uri uri = new Uri("ws://localhost:5000/");
        AppClient app = new AppClient(uri);
        await app.Connect();
        await app.StartListening();
        await app.PerformLogin("maomaoyu", "1234abcd");
    }
}

