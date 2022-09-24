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

        [HttpGet("GetTeams")]
        public IEnumerable<Team> Get()
        {
            return _teamService.GetTeams();
        }
    }
}