using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cryptochat.Server.UserManagement
{
    [ApiController]
    public class UserController : ControllerBase 
    {

        private readonly ILogger logger;
        private readonly IUserService userService;
        
        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [HttpGet]
        [Route("key")]
        public string GetPublicKey(string username)
        {
            return userService.GetPublicKey(username);
        }
    }
}