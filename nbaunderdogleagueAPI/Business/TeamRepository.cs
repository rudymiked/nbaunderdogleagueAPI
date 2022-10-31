using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;
using System;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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
        public TeamRepository(ITeamDataAccess teamDataAccess)
        {
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
            return version switch {
                0 => _teamDataAccess.GetTeamStats().Values.OrderByDescending(team => team.Wins).ToList(),
                1 => _teamDataAccess.GetTeamStatsV1().Result.Values.OrderByDescending(team => team.Wins).ToList(),
                2 => _teamDataAccess.GetTeamStatsV2().Values.OrderByDescending(team => team.Wins).ToList(),
                _ => _teamDataAccess.GetTeamStatsV2().Values.OrderByDescending(team => team.Wins).ToList(),
            };
        }

        public Dictionary<string, TeamStats> TeamStatsDictionary(int version)
        {
            return version switch {
                0 => _teamDataAccess.GetTeamStats(),
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
