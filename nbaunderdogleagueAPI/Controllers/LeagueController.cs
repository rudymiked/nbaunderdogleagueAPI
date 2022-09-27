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

        public LeagueController(ILogger<LeagueController> logger, ILeagueService leagueService)
        {
            _leagueService = leagueService;
            _logger = logger;
        }

        [HttpGet("GetLeagueStandings")]
        public IEnumerable<LeagueStandings> Get()
        {
            return _leagueService.GetLeagueStandings();
        }        
    }
}