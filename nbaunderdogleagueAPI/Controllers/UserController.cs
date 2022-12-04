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
            return !string.IsNullOrEmpty(groupId) ? Ok(_userService.GetUsers(groupId)) : NoContent();
        }

        [HttpPost("UpsertUser")]
        public ActionResult<User> UpsertUser(User user)
        {
            return (user != null && !string.IsNullOrEmpty(user.Group.ToString())) ? _userService.UpsertUser(user) : NoContent();
        }

        [HttpPost("UpdateUser")]
        public ActionResult<User> UpdateUser(User user)
        {
            return (user != null && !string.IsNullOrEmpty(user.Group.ToString())) ? _userService.UpsertUser(user) : NoContent();
        }
    }
}