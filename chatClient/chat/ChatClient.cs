using System;
using System.Net.Http;
using System.Text;
using Cryptochat.Client.Encryption;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

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
                Console.WriteLine("1" );
                var publicKey = "";


                Console.WriteLine("1" );
                using(var client = new HttpClient())
                {
                    var response = client.GetAsync($"http://localhost:5000/key?username=user1").Result;
                    publicKey = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(publicKey);
                }

                var decryptedMessage = encryptionService.DecryptMessage(message, Convert.FromBase64String(publicKey));

                Console.WriteLine("Message: " + decryptedMessage );
            });
        }

        public void Login(string username)
        {
            hub.InvokeAsync("Login", username, Convert.ToBase64String(encryptionService.GetPublicKey()));
        }

        public void SendMessageToUser(string user, string message)
        {
            Console.WriteLine("Getting Public Key from" + user);
            var publicKey = "";

            using(var client = new HttpClient())
            {
                var response = client.GetAsync($"http://localhost:5000/key?username={user}").Result;
                publicKey = response.Content.ReadAsStringAsync().Result;
            }

            Console.WriteLine(publicKey);

            Console.WriteLine("Encrypting Message");
            var encryptedMessage = encryptionService.EncryptMessage(message, Convert.FromBase64String(publicKey));
            Console.WriteLine("Sending Message");

            hub.InvokeAsync("Send", user, encryptedMessage);
        }
    }
}