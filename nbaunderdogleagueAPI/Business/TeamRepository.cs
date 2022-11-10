using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> TeamStatsList(int version);
        List<TeamStats> UpdateTeamStatsManually();
        Dictionary<string, TeamStats> TeamStatsDictionary(int version);
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly ITeamDataAccess _teamDataAccess;
        private readonly ILogger _logger;
        public TeamRepository(ITeamDataAccess teamDataAccess, ILogger<TeamRepository> logger)
        {
            _logger = logger;
            _teamDataAccess = teamDataAccess;
        }

        public List<TeamEntity> GetTeams()
        {
            return _teamDataAccess.GetTeams();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamDataAccess.AddTeams(teamsEntities);
        }

        public List<TeamStats> TeamStatsList(int version)
        {
            List<TeamStats> nbaStatsDataList = _teamDataAccess.GetTeamStats().Values.OrderByDescending(team => team.Wins).ToList();

            if (nbaStatsDataList.Count == 0) {
                _logger.LogError("NBA Stats Data Failed To Load.");
            }

            return version switch {
                0 => nbaStatsDataList.Count > 0 ? nbaStatsDataList : _teamDataAccess.GetTeamStatsV2().Values.OrderByDescending(team => team.Wins).ToList(),
                1 => _teamDataAccess.GetTeamStatsV1().Result.Values.OrderByDescending(team => team.Wins).ToList(),
                2 => _teamDataAccess.GetTeamStatsV2().Values.OrderByDescending(team => team.Wins).ToList(),
                _ => _teamDataAccess.GetTeamStatsV2().Values.OrderByDescending(team => team.Wins).ToList(),
            };
        }

        public Dictionary<string, TeamStats> TeamStatsDictionary(int version)
        {
            Dictionary<string, TeamStats> nbaStatsData = _teamDataAccess.GetTeamStats();

            if (nbaStatsData.Keys.Count == 0) {
                _logger.LogError("NBA Stats Data Failed To Load.");
            }

            return version switch {
                0 => nbaStatsData.Keys.Count > 0 ? nbaStatsData : _teamDataAccess.GetTeamStatsV2(),
                1 => _teamDataAccess.GetTeamStatsV1().Result,
                2 => _teamDataAccess.GetTeamStatsV2(),
                _ => _teamDataAccess.GetTeamStatsV2(),
            };
        }

        public List<TeamStats> UpdateTeamStatsManually()
        {
            return _teamDataAccess.UpdateTeamStatsManually();
        }
    }
}
