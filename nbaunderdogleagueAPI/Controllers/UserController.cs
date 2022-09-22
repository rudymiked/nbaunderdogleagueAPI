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

        public UserController(ILogger<UserController> logger)
        {
            _userService = new UserService();
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<User> Get()
        {
            return _userService.GetUsers();
        }
    }
}