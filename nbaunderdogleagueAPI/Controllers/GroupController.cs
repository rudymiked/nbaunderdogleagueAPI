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

        [HttpGet("GroupStandings")]
        public ActionResult<List<GroupStandings>> GroupStandings(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_groupService.GetGroupStandings(groupId, 0));
            } else {
                return NoContent();
            }
        }

        [HttpGet("GroupStandingsV1")]
        public ActionResult<List<GroupStandings>> GroupStandingsV1(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_groupService.GetGroupStandings(groupId, 1));
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
                return Ok(_groupService.JoinGroup(joinGroupRequest));
            } else {
                return NoContent();
            }
        }

        [HttpPost("LeaveGroup")]
        public ActionResult<string> LeaveGroup(LeaveGroupRequest leaveGroupRequest)
        {
            if (!string.IsNullOrEmpty(leaveGroupRequest.GroupId) && !string.IsNullOrEmpty(leaveGroupRequest.Email)) {
                return Ok(_groupService.LeaveGroup(leaveGroupRequest));
            } else {
                return NoContent();
            }
        }

        [HttpGet("Group")]
        public ActionResult<GroupEntity> GetGroup(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_groupService.GetGroup(groupId));
            } else {
                return NoContent();
            }
        }

        [HttpGet("AllGroupsUserIsInByYear")]
        public ActionResult<List<GroupEntity>> GetAllGroupsUserIsInByYear(string email, int year)
        {
            if (!string.IsNullOrEmpty(email) && year >= AppConstants.MinYear) {
                return Ok(_groupService.GetAllGroupsUserIsInByYear(email, year));
            } else {
                return NoContent();
            }
        }

        [HttpGet("AllGroupsByYear")]
        public ActionResult<List<GroupEntity>> GetAllGroupsByYear(int year, bool includeUser, string email)
        {
            if (year >= AppConstants.MinYear && !string.IsNullOrEmpty(email)) {
                return Ok(_groupService.GetAllGroupsByYear(year, includeUser, email));
            } else {
                return NoContent();
            }
        }
    }
}