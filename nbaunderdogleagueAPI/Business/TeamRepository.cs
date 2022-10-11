using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<CurrentNBAStanding> CurrentNBAStandingsList();
        Dictionary<string, CurrentNBAStanding> GetCurrentNBAStandingsDictionary();
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

        public List<CurrentNBAStanding> CurrentNBAStandingsList()
        {
            return _teamDataAccess.GetCurrentNBAStandings().Result.Values.OrderByDescending(team => team.Win).ToList();
        }

        public Dictionary<string, CurrentNBAStanding> GetCurrentNBAStandingsDictionary()
        {
            return _teamDataAccess.GetCurrentNBAStandings().Result;
        }
    }
}
