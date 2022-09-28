using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ILeagueRepository
    {
        List<LeagueStandings> GetLeagueStandings();
        User DraftTeam(User user);
        List<Draft> SetupDraft();
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
        public List<Draft> SetupDraft()
        {
            return _leagueDataAccess.SetupDraft();
        }
    }
}
