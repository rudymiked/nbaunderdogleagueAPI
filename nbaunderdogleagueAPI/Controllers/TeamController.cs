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
            return Ok(_teamService.TeamStatsList(0));
        }

        [HttpGet("TeamStatsV1")]
        public ActionResult<IEnumerable<TeamStats>> TeamStatsV1()
        {
            return Ok(_teamService.TeamStatsList(1));
        }

        [HttpGet("TeamStatsV2")]
        public ActionResult<IEnumerable<TeamStats>> TeamStatsV2()
        {
            return Ok(_teamService.TeamStatsList(2));
        }

        [HttpGet("UpdateTeamStatsManually")]
        public ActionResult<IEnumerable<TeamStats>> UpdateTeamStatsManually()
        {
            return Ok(_teamService.UpdateTeamStatsManually());
        }

        [HttpGet("TeamsTable")]
        public ActionResult<IEnumerable<TeamEntity>> TeamsTable()
        {
            return Ok(_teamService.GetTeams());
        }        

        [HttpPost("AddTeams")]
        public ActionResult<IEnumerable<TeamEntity>> AddTeams(TeamEntity[] teams)
        {
            return (teams.Length > 0) ? Ok(_teamService.AddTeams(teams.ToList())) : NoContent();
        }
    }
}