using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;  

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> Get()
        {
            return _userService.GetUsers();
        }
    }
}