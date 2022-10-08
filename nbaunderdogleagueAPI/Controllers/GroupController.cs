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
        public ActionResult<List<GroupStandings>> Get(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_groupService.GetGroupStandings(groupId));
            } else {
                return NoContent();
            }

        }

        [HttpPost("CreateGroup")]
        public ActionResult<GroupEntity> CreateGroup(CreateGroupRequest createGroupRequest)
        {
            if (!string.IsNullOrEmpty(createGroupRequest.Name) && !string.IsNullOrEmpty(createGroupRequest.OwnerEmail)) {
                return Ok(_groupService.CreateGroup(createGroupRequest.Name, createGroupRequest.OwnerEmail));
            } else {
                return NoContent();
            }
        }

        [HttpPost("JoinGroup")]
        public ActionResult<string> JoinGroup(JoinGroupRequest joinGroupRequest)
        {
            if (!string.IsNullOrEmpty(joinGroupRequest.GroupId) && !string.IsNullOrEmpty(joinGroupRequest.Email)) {
                return Ok(_groupService.JoinGroup(joinGroupRequest.GroupId, joinGroupRequest.Email));
            } else {
                return NoContent();
            }
        }

        [HttpGet("GetGroup")]
        public ActionResult<GroupEntity> GetGroup(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_groupService.GetGroup(groupId));
            } else {
                return NoContent();
            }
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
        public ActionResult<List<GroupEntity>> GetAllGroupsByYear(int year, bool includeUser, string email)
        {
            return Ok(_groupService.GetAllGroupsByYear(year, includeUser, email));
        }
    }
}