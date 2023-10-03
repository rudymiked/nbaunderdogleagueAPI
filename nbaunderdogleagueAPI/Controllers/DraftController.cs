using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Text.RegularExpressions;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DraftController : ControllerBase
    {
        private readonly ILogger<DraftController> _logger;
        private readonly IDraftService _draftService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DraftController(ILogger<DraftController> logger, IDraftService draftService, IHttpContextAccessor httpContextAccessor)
        {
            _draftService = draftService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("DraftTeam")]
        public ActionResult<string> DraftTeam(User user)
        {
            return (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Team)) ? Ok(_draftService.DraftTeam(user)) : NoContent();
        }

        [HttpPost("SetupDraft")]
        public ActionResult<List<DraftEntity>> SetupDraft(SetupDraftRequest setupDraftRequest)
        {
            return !string.IsNullOrEmpty(setupDraftRequest.GroupId) ? _draftService.SetupDraft(setupDraftRequest) : NoContent();
        }

        [HttpGet("DraftedTeams")]
        public ActionResult<List<UserEntity>> DraftedTeams(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_draftService.DraftedTeams(groupId)) : NoContent();
        }

        [HttpGet("Draft")]
        public ActionResult<List<DraftEntity>> GetDraft(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_draftService.GetDraft(groupId)) : NoContent();
        }

        [HttpGet("AvailableTeamsToDraft")]
        public ActionResult<List<TeamEntity>> GetAvailableTeamsToDraft(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_draftService.GetAvailableTeamsToDraft(groupId)) : NoContent();
        }

        [HttpGet("DraftResults")]
        public ActionResult<List<DraftResults>> GetDraftResults(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_draftService.GetDraftResults(groupId)) : NoContent();
        }

        [HttpPost("DraftLate")]
        public ActionResult<string> DraftLate(DraftLateRequest draftLateRequest)
        {
            if (!string.IsNullOrEmpty(draftLateRequest.GroupId)) {
                return "Group is blank";
            }

            if (!string.IsNullOrEmpty(draftLateRequest.Email)) {
                return "Email is blank";
            }

            if (!string.IsNullOrEmpty(draftLateRequest.AdminEmail)) {
                return "Admin Email is blank";
            }

            if (!string.IsNullOrEmpty(draftLateRequest.Team)) {
                return "Team is blank";
            }

            return Ok(_draftService.DraftLate(draftLateRequest));
        }
    }
}