using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserEntity> Get(string leagueId)
        {
            return _userService.GetUsers(leagueId);
        }

        [HttpPost("AddUser")]
        public User AddUser(User user)
        {
            return _userService.AddUser(user);
        }
    }
}