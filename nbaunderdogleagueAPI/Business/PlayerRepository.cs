using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;
using static nbaunderdogleagueAPI.Models.PlayerStatistics.PlayerStatistics;

namespace nbaunderdogleagueAPI.Business
{
    public interface IPlayerRepository
    {
        public PlayerResponse GetAllPlayerStatsFromRapidAPI(int season);
        public PlayerResponse GetPlayerStatsPerTeamFromRapidAPI(int teamId, int season);
        public List<PlayerStatisticsEntity> UpdatePlayerStatsFromRapidAPI(int teamId = 0, int season = 0);
        public List<PlayerStatisticsEntity> GetPlayerStatistics(string playerName = "");
        public List<AdvancedPlayerStats> GetAdvancedPlayerStatistics(string playerName = "");
    }
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IPlayerDataAccess _playerDataAccess;
        public PlayerRepository(IPlayerDataAccess playerDataAccess)
        {
            _playerDataAccess = playerDataAccess;
        }
        public PlayerResponse GetAllPlayerStatsFromRapidAPI(int season)
        {
            return _playerDataAccess.GetAllPlayerStatsFromRapidAPI(season);
        }
        public PlayerResponse GetPlayerStatsPerTeamFromRapidAPI(int teamId, int season)
        {
            return _playerDataAccess.GetPlayerStatsPerTeamFromRapidAPI(teamId, season);
        }
        public List<PlayerStatisticsEntity> UpdatePlayerStatsFromRapidAPI(int teamId = 0, int season = 0)
        {
            return _playerDataAccess.UpdatePlayerStatsFromRapidAPI(teamId, season);
        }
        public List<PlayerStatisticsEntity> GetPlayerStatistics(string playerName = "")
        {
            return _playerDataAccess.GetPlayerStatistics(playerName);
        }
        public List<AdvancedPlayerStats> GetAdvancedPlayerStatistics(string playerName = "")
        {
            return _playerDataAccess.GetAdvancedPlayerStatistics(playerName);
        }
    }
}
