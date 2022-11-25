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
        public ActionResult SeasonArchive(string groupId)
        {
            List<SeasonArchiveEntity> seasonArchiveEntities = _archiveService.GetSeasonArchive(groupId);

            return seasonArchiveEntities.Count > 0 ? Ok(seasonArchiveEntities) : NoContent();
        }

        [HttpPost("ArchiveCurrentSeason")]
        public ActionResult ArchiveCurrentSeason(string groupId)
        {
            List<SeasonArchiveEntity> seasonArchiveEntities = _archiveService.ArchiveCurrentSeason(groupId);

            return seasonArchiveEntities.Count > 0 ? Ok(seasonArchiveEntities) : NoContent();
        }
    }
}