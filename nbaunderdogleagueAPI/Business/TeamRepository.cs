using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<Team> GetTeams();
        List<TeamsEntity> GetTeamsEntity();
        List<TeamsEntity> AddTeams(List<TeamsEntity> teamsEntities);
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly ITeamDataAccess _teamDataAccess;
        public TeamRepository(ITeamDataAccess teamDataAccess)
        {
            _teamDataAccess = teamDataAccess;
        }
        public List<Team> GetTeams()
        {
            return _teamDataAccess.GetTeamData();
        }

        public List<TeamsEntity> GetTeamsEntity()
        {
            return _teamDataAccess.GetTeamsAsync().Result;
        }

        public List<TeamsEntity> AddTeams(List<TeamsEntity> teamsEntities)
        {
            return _teamDataAccess.AddTeamsAsync(teamsEntities).Result;
        }
    }
}
