using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NBAController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly INBAService _nbaService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NBAController(ILogger<NBAController> logger, INBAService nbaService, IHttpContextAccessor httpContextAccessor)
        {
            _nbaService = nbaService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("UpdateTeamStatsFromRapidAPI")]
        public ActionResult<IEnumerable<TeamStats>> UpdateTeamStatsFromRapidAPI()
        {
            return Ok(_nbaService.UpdateTeamStatsFromRapidAPI());
        }

        [HttpGet("UpdateGamesFromRapidAPI")]
        public ActionResult<IEnumerable<NBAGameEntity>> UpdateGamesFromRapidAPI()
        {
            return Ok(_nbaService.UpdateGamesFromRapidAPI());
        }

        [HttpGet("NBAScoreboard")]
        public ActionResult<IEnumerable<NBAGameEntity>> NBAScoreboard()
        {
            return Ok(_nbaService.NBAScoreboard());
        }
    }
}
