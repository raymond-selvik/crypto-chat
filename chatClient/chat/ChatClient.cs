using System;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptochat.Client
{
    public class ChatClient
    {
        HubConnection hub;
        MessageEncryptionService encryptionService;

        public ChatClient(string hubUrl)
        {
            hub = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            hub.StartAsync();

            encryptionService = new MessageEncryptionService();

            hub.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine("Message: " + message );
            });
        }

        public void Login(string username)
        {
            hub.InvokeAsync("Login", username, Encoding.UTF8.GetString(encryptionService.GetPublicKey()));
        }

        public void SendMessageToUser(string user, string message)
        {
            hub.InvokeAsync("SendMessageToUser", message);
        }
    }
}