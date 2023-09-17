using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Services;
using System.Security.Claims;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;

            _user = _httpContextAccessor.HttpContext.User;
        }

        [Authorize]
        [HttpGet("Me")]
        public ActionResult Me()
        {
            return Ok(_user);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> Get(int id)
        {
            return User.Identity.Name;
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