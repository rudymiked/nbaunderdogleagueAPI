using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<Team> GetTeams();
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly ITeamDataAccess _teamDataAccess;
        public TeamRepository()
        {
            _teamDataAccess = new TeamDataAccess();
        }
        public List<Team> GetTeams()
        {
            return _teamDataAccess.GetTeamData();
        }
    }
}
