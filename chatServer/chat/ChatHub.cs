using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace chatServer
{
    public class ChatHub : Hub
    {
        List<string> users = new List<string>();

        public async Task Send(string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("Send", message);
        }
    }
}