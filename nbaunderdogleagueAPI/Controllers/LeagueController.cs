using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeagueController : ControllerBase
    {
        private readonly ILogger<LeagueController> _logger;
        private readonly ILeagueService _leagueService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeagueController(ILogger<LeagueController> logger, ILeagueService leagueService, IHttpContextAccessor httpContextAccessor)
        {
            _leagueService = leagueService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetLeagueStandings")]
        public IEnumerable<LeagueStandings> Get()
        {
            return _leagueService.GetLeagueStandings();
        }

        [HttpPost("DraftTeam")]
        public User DraftTeam(User user)
        {
            return _leagueService.DraftTeam(user);
        }

        [HttpPost("SetupDraft")]
        public List<Draft> SetupDraft(string id)
        {
            return _leagueService.SetupDraft(id);
        }

        [HttpPost("CreateLeague")]
        public League CreateLeague(string name, string ownerEmail)
        {
            return _leagueService.CreateLeague(name, ownerEmail);
        }

        [HttpPost("JoinLeague")]
        public League JoinLeague(string id, string email)
        {
            return _leagueService.JoinLeague(id, email);
        }
    }
}