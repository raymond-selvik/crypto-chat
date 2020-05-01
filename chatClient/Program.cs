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

            Console.WriteLine($"Welcome to Crypto-Chat {username}!");

            while (true)
            {
                Console.WriteLine("Who do you want to send a message to?");
                var to = Console.ReadLine();

                Console.WriteLine("Type in your message:");
                var message = Console.ReadLine();
                chatClient.SendMessageToUser(to, message);
            }
        }
    }
}
