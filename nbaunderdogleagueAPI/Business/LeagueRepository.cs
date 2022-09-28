using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ILeagueRepository
    {
        List<LeagueStandings> GetLeagueStandings();
        User DraftTeam(User user);
        List<Draft> SetupDraft(string id);
        League CreateLeague(string name, string ownerEmail);
    }

    public class LeagueRepository : ILeagueRepository
    {
        private readonly ILeagueDataAccess _leagueDataAccess;
        public LeagueRepository(ILeagueDataAccess leagueDataAccess)
        {
            _leagueDataAccess = leagueDataAccess;
        }
        public List<LeagueStandings> GetLeagueStandings()
        {
            return _leagueDataAccess.GetLeagueStandings();
        }
        public User DraftTeam(User user)
        {
            return _leagueDataAccess.DraftTeam(user);
        }
        public List<Draft> SetupDraft(string id)
        {
            return _leagueDataAccess.SetupDraft(id);
        }        
        public League CreateLeague(string name, string ownerEmail)
        {
            return _leagueDataAccess.CreateLeague(name, ownerEmail);
        }
    }
}
