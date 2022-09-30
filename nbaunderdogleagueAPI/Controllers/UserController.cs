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
        public ActionResult<IEnumerable<UserEntity>> Get(string groupId)
        {
            if (groupId != null && groupId != string.Empty) {
                return Ok(_userService.GetUsers(groupId));
            }

            return NoContent();
        }

        [HttpPost("AddUser")]
        public ActionResult<User> AddUser(User user)
        {
            if (user != null && user.Group.ToString() != string.Empty) {
                return _userService.AddUser(user);
            }

            return NoContent();
        }
    }
}