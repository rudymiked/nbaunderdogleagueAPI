using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [Authorize(Policy = AppConstants.AudiencePolicy)]
        [HttpGet("TeamStatsDictionaryFromJSON")]
        public ActionResult<IEnumerable<TeamStats>> TeamStatsDictionaryFromJSON()
        {
            return Ok(_teamService.TeamStatsDictionaryFromJSON());
        }

        [HttpGet("TeamStatsListFromNBAdotCom")]
        public ActionResult<IEnumerable<TeamStats>> TeamStatsListFromNBAdotCom()
        {
            return Ok(_teamService.TeamStatsListFromNBAdotCom());
        }

        //[Authorize(Policy = AppConstants.DefaultAuthPolicy)]
        //[Authorize(Policy = AppConstants.AudiencePolicy)]
        [HttpGet("TeamStatsListFromStorage")]
        public ActionResult<IEnumerable<TeamStats>> TeamStatsListFromStorage()
        {
            return Ok(_teamService.TeamStatsListFromStorage());
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

        [HttpPost("UpdateTeamPlayoffWins")]
        public ActionResult<string> UpdateTeamPlayoffWins(TeamStats teamStats)
        {
            return Ok(_teamService.UpdateTeamPlayoffWins(teamStats));
        }
    }
}