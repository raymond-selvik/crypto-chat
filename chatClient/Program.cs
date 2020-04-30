using System;   
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptochat.Client
{
    class Program
    {
        static ChatClient chatClient;
        static void Main(string[] args)
        {
            var username = args[0];

            chatClient = new ChatClient("http://localhost:5000/chathub");
            chatClient.Login(username);

            string message = Console.ReadLine();

            while (message != string.Empty)
            {
                chatClient.SendMessageToUser(message, message);
                message = Console.ReadLine();
            }
        }
    }
}
