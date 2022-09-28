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
        public IEnumerable<UserEntity> Get()
        {
            return _userService.GetUsers();
        }
        [HttpPost("AddUsers")]
        public IEnumerable<UserEntity> AddUsers(List<UserEntity> userEntities)
        {
            return _userService.AddUsers(userEntities);
        }
    }
}