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

        [HttpGet("TeamStats")]
        public ActionResult<IEnumerable<TeamStats>> TeamStats()
        {
            return Ok(_teamService.TeamStatsList());
        }

        [HttpGet("TeamsTable")]
        public ActionResult<IEnumerable<TeamEntity>> TeamsTable()
        {
            return Ok(_teamService.GetTeams());
        }

        [HttpPost("AddTeams")]
        public ActionResult<IEnumerable<TeamEntity>> AddTeams(TeamEntity[] teams)
        {
            if (teams.Length > 0) {
                return Ok(_teamService.AddTeams(teams.ToList()));
            } else {
                return NoContent();
            }
        }
    }
}