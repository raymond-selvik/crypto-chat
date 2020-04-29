using System;   
using Microsoft.AspNetCore.SignalR.Client;

namespace chatClient
{
    class Program
    {
        static HubConnection hub;
        static void Main(string[] args)
        {
            hub = new HubConnectionBuilder().WithUrl("http://localhost:5000/chathub").Build();
            
            hub.StartAsync().GetAwaiter().GetResult();

            hub.On<string>("Send", (message) =>
            {
                Console.WriteLine("Message: " + message );
            });

            string message = Console.ReadLine();

            Console.WriteLine("Hello World!");

            while (message != string.Empty)
            {
                hub.InvokeAsync("Send", message );
                message = Console.ReadLine();
            }
        }
    }
}
