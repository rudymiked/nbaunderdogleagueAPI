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

        [HttpGet("GetStandings")]
        public IEnumerable<Standings> Get()
        {
            return _teamService.GetStandings();
        }        

        [HttpGet("GetCurrentNBAStandings")]
        public IEnumerable<CurrentNBAStandings> GetCurrentNBAStandings()
        {
            return _teamService.GetCurrentNBAStandings();
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