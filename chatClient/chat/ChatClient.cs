using System;
using System.Net.Http;
using Cryptochat.Client.Encryption;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptochat.Client
{
    public class ChatClient
    {
        HubConnection hub;
        MessageEncryptionService encryptionService;

        string username;

        public ChatClient(string hubUrl)
        {
            hub = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            hub.StartAsync();

            encryptionService = new MessageEncryptionService();

            hub.On<string, string>("ReceiveMessage", (from, message) =>
            {
                var senderKey = GetUserPublicKey(from);


                var decryptedMessage = encryptionService.DecryptMessage(message, senderKey);

                Console.WriteLine($"{from}: {decryptedMessage}");
            });
        }

        public void Login(string username)
        {
            this.username = username;

            hub.InvokeAsync("Login", username, encryptionService.GetPublicKey());
        }

        public void SendMessageToUser(string user, string message)
        {
            var receiverKey = GetUserPublicKey(user);

            Console.WriteLine("Encrypting Message");
            var encryptedMessage = encryptionService.EncryptMessage(message, receiverKey);
            Console.WriteLine("Sending Message");

            hub.InvokeAsync("Send", username, user, encryptedMessage);
        }

        string GetUserPublicKey(string username)
        {
            using(var client = new HttpClient())
            {
                var response = client.GetAsync($"http://localhost:5000/key?username={username}").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}