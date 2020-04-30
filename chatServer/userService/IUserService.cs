namespace Cryptochat.Server.UserManagement
{
    public interface IUserService
    {
        void StoreUser(User user);

        User GetUser(string username);
        string GetPublicKey(string username);
    }
}
