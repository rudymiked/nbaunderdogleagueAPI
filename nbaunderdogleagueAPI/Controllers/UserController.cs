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

        [HttpGet("Users")]
        public ActionResult<IEnumerable<UserEntity>> Users(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_userService.GetUsers(groupId));
            }

            return NoContent();
        }

        [HttpPost("UpsertUser")]
        public ActionResult<User> UpsertUser(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.Group.ToString())) {
                return _userService.UpsertUser(user);
            }

            return NoContent();
        }
        [HttpPost("UpdateUser")]
        public ActionResult<User> UpdateUser(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.Group.ToString())) {
                return _userService.UpsertUser(user);
            }

            return NoContent();
        }
    }
}