using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptochat.Client
{
    public class ChatClient
    {
        HubConnection hub;

        public ChatClient(string hubUrl)
        {
            hub = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            hub.StartAsync();

            hub.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine("Message: " + message );
            });
        }

        public void Login(string username)
        {
            hub.InvokeAsync("Login", username);
        }

        public void SendMessageToUser(string user, string message)
        {
            hub.InvokeAsync("SendMessageToUser", message);
        }
    }
}