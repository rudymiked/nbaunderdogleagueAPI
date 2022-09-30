using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        private readonly IGroupService _groupService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupController(ILogger<GroupController> logger, IGroupService groupService, IHttpContextAccessor httpContextAccessor)
        {
            _groupService = groupService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetGroupStandings")]
        public IEnumerable<GroupStandings> Get(string groupId)
        {
            return _groupService.GetGroupStandings(groupId);
        }

        [HttpPost("CreateGroup")]
        public GroupEntity CreateGroup(string name, string ownerEmail)
        {
            return _groupService.CreateGroup(name, ownerEmail);
        }

        [HttpPost("JoinGroup")]
        public string JoinGroup(string id, string email)
        {
            return _groupService.JoinGroup(id, email);
        }

        [HttpGet("GetGroup")]
        public GroupEntity GetGroup(string groupId)
        {
            return _groupService.GetGroup(groupId);
        }

        [HttpGet("GetAllGroupsUserIsInByYear")]
        public List<GroupEntity> GetAllGroupsUserIsInByYear(int year)
        {
            return _groupService.GetAllGroupsByYear(year);
        }

        [HttpGet("GetAllGroupsByYear")]
        public List<GroupEntity> GetAllGroupsByYear(int year)
        {
            return _groupService.GetAllGroupsByYear(year);
        }
    }
}