using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cryptochat.Server.UserManagement;

namespace Cryptochat.Server
{
    public class ChatHub : Hub
    {
        private readonly ILogger logger;
        private readonly IUserService userService;

        public ChatHub(ILogger<ChatHub> logger, IUserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        public void Login(string username, string publicKey)
        {
            User user = new User
            {
                Name = username,
                Id = Context.ConnectionId,
                PublicKey = publicKey
            };

            userService.StoreUser(user);

            logger.LogInformation($"Added user: {user.Name} {user.Id}");
        }
        public void Send(string from, string to, string message)
        {
            logger.LogInformation(message);
            
            User receiver = userService.GetUser(to);
            Clients.Client(receiver.Id).SendAsync("ReceiveMessage", from, message);
        }
    }
}