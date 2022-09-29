using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DraftController : ControllerBase
    {
        private readonly ILogger<LeagueController> _logger;
        private readonly IDraftService _draftService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DraftController(ILogger<LeagueController> logger, IDraftService draftService, IHttpContextAccessor httpContextAccessor)
        {
            _draftService = draftService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("DraftTeam")]
        public Dictionary<User, List<string>> DraftTeam(User user)
        {
            return _draftService.DraftTeam(user);
        }

        [HttpPost("SetupDraft")]
        public List<DraftEntity> SetupDraft(string id)
        {
            return _draftService.SetupDraft(id);
        }
    }
}