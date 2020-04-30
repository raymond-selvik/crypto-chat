using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Cryptochat.Server.UserManagement
{
    public class UserService : IUserService
    {

        static ConcurrentDictionary<string, User> users = new ConcurrentDictionary<string,User>();
        private readonly ILogger logger; 
        //private readonly UserDbContext userDbContext;

        public UserService(ILogger<UserService> logger)
        {
            this.logger = logger;
        }

        public string GetPublicKey(string username)
        {
            var user = users[username];
            return user.PublicKey;
        }

        public User GetUser(string username)
        {
            var user = users[username];
            return user;
        }

        public void StoreUser(User user)
        {
            users.TryAdd(user.Name, user);
        }

        
    }
}