using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchiveController : ControllerBase
    {
        private readonly ILogger<ArchiveController> _logger;
        private readonly IArchiveService _archiveService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArchiveController(ILogger<ArchiveController> logger, IArchiveService archiveService, IHttpContextAccessor httpContextAccessor)
        {
            _archiveService = archiveService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("SeasonArchive")]
        public ActionResult<List<SeasonArchiveEntity>> SeasonArchive(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_archiveService.GetSeasonArchive(groupId)) : NoContent();
        }

        [HttpGet("ArchiveSummary")]
        public ActionResult<List<ArchiveSummary>> ArchiveSummary(string email)
        {
            return !string.IsNullOrEmpty(email) ? Ok(_archiveService.GetArchiveSummary(email)) : NoContent();
        }

        [HttpPost("ArchiveCurrentSeason")]
        public ActionResult<List<SeasonArchiveEntity>> ArchiveCurrentSeason(string groupId)
        {
            return !string.IsNullOrEmpty(groupId) ? Ok(_archiveService.ArchiveCurrentSeason(groupId)) : NoContent();
        }
    }
}