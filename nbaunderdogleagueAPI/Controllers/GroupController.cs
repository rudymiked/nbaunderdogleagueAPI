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
        public ActionResult<IEnumerable<GroupStandings>> Get(string groupId)
        {
            return Ok(_groupService.GetGroupStandings(groupId));
        }

        [HttpPost("CreateGroup")]
        public ActionResult<GroupEntity> CreateGroup(Group newGroup)
        {
            if (!string.IsNullOrEmpty(newGroup.Name) && !string.IsNullOrEmpty(newGroup.Owner)) {
                return Ok(_groupService.CreateGroup(newGroup.Name, newGroup.Owner));
            } else {
                return NoContent();
            }
        }

        [HttpPost("JoinGroup")]
        public ActionResult<string> JoinGroup(string id, string email)
        {
            return Ok(_groupService.JoinGroup(id, email));
        }

        [HttpGet("GetGroup")]
        public ActionResult<GroupEntity> GetGroup(string groupId)
        {
            return Ok(_groupService.GetGroup(groupId));
        }

        [HttpGet("GetAllGroupsUserIsInByYear")]
        public ActionResult<List<GroupEntity>> GetAllGroupsUserIsInByYear(string email, int year)
        {
            if (!string.IsNullOrEmpty(email) && year > AppConstants.MinYear) {
                return Ok(_groupService.GetAllGroupsUserIsInByYear(email, year));
            } else {
                return NoContent();
            }
        }

        [HttpGet("GetAllGroupsByYear")]
        public ActionResult<List<GroupEntity>>GetAllGroupsByYear(int year)
        {
            return Ok(_groupService.GetAllGroupsByYear(year));
        }
    }
}