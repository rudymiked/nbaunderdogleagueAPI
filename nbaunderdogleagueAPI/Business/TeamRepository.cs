using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<Standings> GetStandings();
        List<TeamEntity> GetTeamsEntity();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<CurrentNBAStanding> GetCurrentNBAStandings();
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly ITeamDataAccess _teamDataAccess;
        public TeamRepository(ITeamDataAccess teamDataAccess)
        {
            _teamDataAccess = teamDataAccess;
        }
        public List<Standings> GetStandings()
        {
            return _teamDataAccess.GetStandingsData();
        }

        public List<TeamEntity> GetTeamsEntity()
        {
            return _teamDataAccess.GetTeams();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamDataAccess.AddTeams(teamsEntities);
        }        
        public List<CurrentNBAStanding> GetCurrentNBAStandings()
        {
            return _teamDataAccess.GetCurrentNBAStandings().Result.Values.ToList();
        }
    }
}
