using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> TeamStatsList();
        Dictionary<string, TeamStats> GetTeamStatsDictionary();
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

        public List<TeamStats> TeamStatsList()
        {
            return _teamDataAccess.GetTeamStats().Values.OrderByDescending(team => team.Wins).ToList();
        }

        public Dictionary<string, TeamStats> GetTeamStatsDictionary()
        {
            return _teamDataAccess.GetTeamStats();
        }
    }
}
