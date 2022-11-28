using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> TeamStatsList(int version);
        Dictionary<string, TeamStats> TeamStatsDictionary(int version);
        List<TeamStats> UpdateTeamStatsManually();
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public List<TeamEntity> GetTeams()
        {
            return _teamRepository.GetTeams();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamRepository.AddTeams(teamsEntities);
        }

        public List<TeamStats> TeamStatsList(int version)
        {
            return _teamRepository.TeamStatsList(version);
        }

        public Dictionary<string, TeamStats> TeamStatsDictionary(int version)
        {
            return _teamRepository.TeamStatsDictionary(version);
        }

        public List<TeamStats> UpdateTeamStatsManually()
        {
            return _teamRepository.UpdateTeamStatsManually();
        }

        public List<TeamStats> UpdateTeamStatsFromRapidAPI()
        {
            return _teamRepository.UpdateTeamStatsFromRapidAPI();
        }
    }
}
