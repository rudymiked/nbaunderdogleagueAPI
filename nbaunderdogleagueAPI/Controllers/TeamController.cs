using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ILogger<TeamController> _logger;
        private readonly ITeamService _teamService;  

        public TeamController(ILogger<TeamController> logger, ITeamService teamService)
        {
            _teamService = teamService;
            _logger = logger;
        }    

        [HttpGet("GetCurrentNBAStandings")]
        public IEnumerable<CurrentNBAStanding> GetCurrentNBAStandings()
        {
            return _teamService.GetCurrentNBAStandingsList();
        }

        [HttpGet("GetTeamsTable")]
        public IEnumerable<TeamEntity> GetTeamsTable()
        {
            return _teamService.GetTeamsEntity();
        }

        [HttpPost("AddTeams")]
        public IEnumerable<TeamEntity> AddTeams(TeamEntity[] teams)
        {
            return _teamService.AddTeams(teams.ToList());
        }
    }
}