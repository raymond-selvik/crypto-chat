using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Cryptochat.Server
{
    public class ChatHub : Hub
    {
        static ConcurrentDictionary<string, User> users = new ConcurrentDictionary<string,User>();


        public void Login(string username)
        {
            User user = new User
            {
                name = username,
                id = Context.ConnectionId
            };

            if(!users.TryAdd(user.name, user))
            {
                Console.WriteLine("Failed to add user");
            }

            Console.WriteLine($"Added user: {user.name} {user.id}" + users.IsEmpty);
        }
        public async Task Send(string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToUser(string user)
        {
            Console.WriteLine("Private Chat Request" + users.IsEmpty);
            User receiver = new User();
            users.TryGetValue(user, out receiver);
            Console.WriteLine("Sending to" + receiver.id);

            
            return Clients.Client(receiver.id).SendAsync("ReceiveMessage", "From server private");
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}