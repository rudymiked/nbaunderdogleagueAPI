using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;
using static nbaunderdogleagueAPI.Models.PlayerStatistics.PlayerStatistics;

namespace nbaunderdogleagueAPI.Services
{
    public interface IPlayerService
    {
        public PlayerResponse GetAllPlayerStatsFromRapidAPI(int season);
        public PlayerResponse GetPlayerStatsPerTeamFromRapidAPI(int teamId, int season);
        public List<PlayerStatisticsEntity> UpdatePlayerStatsFromRapidAPI(int teamId = 0, int season = 0);
        public List<PlayerStatisticsEntity> GetPlayerStatistics(string playerName = "");
        public List<AdvancedPlayerStats> GetAdvancedPlayerStatistics(string playerName = "");
    }
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        public PlayerResponse GetAllPlayerStatsFromRapidAPI(int season)
        {
            return _playerRepository.GetAllPlayerStatsFromRapidAPI(season);
        }
        public PlayerResponse GetPlayerStatsPerTeamFromRapidAPI(int teamId, int season)
        {
            return _playerRepository.GetPlayerStatsPerTeamFromRapidAPI(teamId, season);
        }
        public List<PlayerStatisticsEntity> UpdatePlayerStatsFromRapidAPI(int teamId = 0, int season = 0)
        {
            return _playerRepository.UpdatePlayerStatsFromRapidAPI(teamId, season);
        }
        public List<PlayerStatisticsEntity> GetPlayerStatistics(string playerName = "")
        {
            return _playerRepository.GetPlayerStatistics(playerName);
        }
        public List<AdvancedPlayerStats> GetAdvancedPlayerStatistics(string playerName = "")
        {
            return _playerRepository.GetAdvancedPlayerStatistics(playerName);
        }
    }
}
