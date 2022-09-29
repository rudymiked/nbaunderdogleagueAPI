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
        public IEnumerable<LeagueStandings> Get(string leagueId)
        {
            return _leagueService.GetLeagueStandings(leagueId);
        }

        [HttpPost("CreateLeague")]
        public LeagueInfo CreateLeague(string name, string ownerEmail)
        {
            return _leagueService.CreateLeague(name, ownerEmail);
        }

        [HttpPost("JoinLeague")]
        public string JoinLeague(string id, string email)
        {
            return _leagueService.JoinLeague(id, email);
        }

        [HttpPost("GetLeague")]
        public LeagueEntity GetLeague(string leagueId)
        {
            return _leagueService.GetLeague(leagueId);
        }
    }
}