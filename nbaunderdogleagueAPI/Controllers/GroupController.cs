using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Security.Claims;

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

            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            _logger.LogInformation("User: " + user.Identity.Name);
        }

        [HttpGet("GroupStandings")]
        public ActionResult<List<GroupStandings>> GroupStandings(string groupId)
        {
            return (!string.IsNullOrEmpty(groupId)) ? Ok(_groupService.GetGroupStandings(groupId, 0)) : NoContent();
        }

        [HttpGet("GroupStandingsV1")]
        public ActionResult<List<GroupStandings>> GroupStandingsV1(string groupId)
        {
            return (!string.IsNullOrEmpty(groupId)) ? Ok(_groupService.GetGroupStandings(groupId, 1)) : NoContent();
        }

        [HttpGet("GroupStandingsV2")]
        public ActionResult<List<GroupStandings>> GroupStandingsV2(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_groupService.GetGroupStandings(groupId, 2)) : NoContent();
        }

        [HttpPost("CreateGroup")]
        public ActionResult<GroupEntity> CreateGroup(CreateGroupRequest createGroupRequest)
        {
            return !string.IsNullOrEmpty(createGroupRequest.Name) && !string.IsNullOrEmpty(createGroupRequest.OwnerEmail)
                ? Ok(_groupService.CreateGroup(createGroupRequest.Name, createGroupRequest.OwnerEmail))
                : NoContent();
        }

        [HttpPost("JoinGroup")]
        public ActionResult<string> JoinGroup(JoinGroupRequest joinGroupRequest)
        {
            return (!string.IsNullOrEmpty(joinGroupRequest.GroupId) && !string.IsNullOrEmpty(joinGroupRequest.Email))
                ? Ok(_groupService.JoinGroup(joinGroupRequest))
                : NoContent();
        }

        [HttpPost("LeaveGroup")]
        public ActionResult<string> LeaveGroup(LeaveGroupRequest leaveGroupRequest)
        {
            return (!string.IsNullOrEmpty(leaveGroupRequest.GroupId) && !string.IsNullOrEmpty(leaveGroupRequest.Email))
                ? Ok(_groupService.LeaveGroup(leaveGroupRequest))
                : NoContent();
        }

        [HttpGet("Group")]
        public ActionResult<GroupEntity> GetGroup(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_groupService.GetGroup(groupId)) : NoContent();
        }

        [HttpGet("AllGroupsUserIsInByYear")]
        public ActionResult<List<GroupEntity>> GetAllGroupsUserIsInByYear(string email, int year)
        {
            return !string.IsNullOrEmpty(email) && year >= AppConstants.MinYear
                ? Ok(_groupService.GetAllGroupsUserIsInByYear(email, year))
                : NoContent();
        }

        [HttpGet("AllGroupsByYear")]
        public ActionResult<List<GroupEntity>> GetAllGroupsByYear(int year, bool includeUser, string email)
        {
            return year >= AppConstants.MinYear && !string.IsNullOrEmpty(email)
                ? Ok(_groupService.GetAllGroupsByYear(year, includeUser, email))
                : NoContent();
        }
    }
}