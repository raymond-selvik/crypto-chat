using Microsoft.EntityFrameworkCore;

namespace Cryptochat.Server.UserManagement
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users {get; set;}
        
    }
}