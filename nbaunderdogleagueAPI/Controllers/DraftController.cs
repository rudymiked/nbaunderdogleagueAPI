using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DraftController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        private readonly IDraftService _draftService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DraftController(ILogger<GroupController> logger, IDraftService draftService, IHttpContextAccessor httpContextAccessor)
        {
            _draftService = draftService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("DraftTeam")]
        public ActionResult<string> DraftTeam(User user)
        {
            if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Team)) {
                return Ok(_draftService.DraftTeam(user));
            } else {
                return NoContent();
            }
        }

        [HttpPost("SetupDraft")]
        public ActionResult<List<DraftEntity>> SetupDraft(SetupDraftRequest setupDraftRequest)
        {
            if (!string.IsNullOrEmpty(setupDraftRequest.GroupId)) {
                return _draftService.SetupDraft(setupDraftRequest);
            } else {
                return NoContent();
            }
        }

        [HttpGet("DraftedTeams")]
        public ActionResult<List<UserEntity>> DraftedTeams(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_draftService.DraftedTeams(groupId));
            } else {
                return NoContent();
            }
        }

        [HttpGet("Draft")]
        public ActionResult<List<DraftEntity>> GetDraft(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_draftService.GetDraft(groupId));
            } else {
                return NoContent();
            }
        }

        [HttpGet("AvailableTeamsToDraft")]
        public ActionResult<List<TeamEntity>> GetAvailableTeamsToDraft(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId)) {
                return Ok(_draftService.GetAvailableTeamsToDraft(groupId));
            } else {
                return NoContent();
            }
        }
    }
}