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
        public Dictionary<User, string> DraftTeam(User user)
        {
            return _draftService.DraftTeam(user);
        }

        [HttpPost("SetupDraft")]
        public List<DraftEntity> SetupDraft(string groupId)
        {
            return _draftService.SetupDraft(groupId);
        }

        [HttpGet("DraftedTeams")]
        public List<UserEntity> DraftedTeams(string groupId)
        {
            return _draftService.DraftedTeams(groupId);
        }

        [HttpGet("GetDraft")]
        public List<DraftEntity> GetDraft(string groupId)
        {
            return _draftService.GetDraft(groupId);
        }

        [HttpGet("GetAvailableTeamsToDraft")]
        public List<TeamEntity> GetAvailableTeamsToDraft(string groupId)
        {
            return _draftService.GetAvailableTeamsToDraft(groupId);
        }
    }
}