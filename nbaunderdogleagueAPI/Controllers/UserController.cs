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
        public IEnumerable<UserEntity> Get(string groupId)
        {
            return _userService.GetUsers(groupId);
        }

        [HttpPost("AddUser")]
        public User AddUser(User user)
        {
            return _userService.AddUser(user);
        }
    }
}