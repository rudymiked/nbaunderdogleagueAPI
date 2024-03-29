﻿using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlayerController(IPlayerService playerService, IHttpContextAccessor httpContextAccessor)
        {
            _playerService = playerService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("Statistics")]
        public ActionResult<IEnumerable<PlayerStatisticsEntity>> Statistics()
        {
            return Ok(_playerService.GetPlayerStatistics());
        }

        [HttpGet("UpdatePlayerStatsFromRapidAPI")]
        public ActionResult<IEnumerable<PlayerStatisticsEntity>> UpdatePlayerStatsFromRapidAPI()
        {
            return Ok(_playerService.UpdatePlayerStatsFromRapidAPI());
        }
    }
}
