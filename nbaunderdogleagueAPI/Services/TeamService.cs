using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> TeamStatsList();
        List<TeamStats> TeamStatsListV1();
        Dictionary<string, TeamStats> GetTeamStatsDictionary();
        Dictionary<string, TeamStats> GetTeamStatsDictionaryV1();
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

        public List<TeamStats> TeamStatsList()
        {
            return _teamRepository.TeamStatsList();
        }

        public Dictionary<string, TeamStats> GetTeamStatsDictionary()
        {
            return _teamRepository.GetTeamStatsDictionary();
        }
        public List<TeamStats> TeamStatsListV1()
        {
            return _teamRepository.TeamStatsListV1();
        }

        public Dictionary<string, TeamStats> GetTeamStatsDictionaryV1()
        {
            return _teamRepository.GetTeamStatsDictionaryV1();
        }
    }
}
