using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ILeagueRepository
    {
        List<LeagueStandings> GetLeagueStandings(); 
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

    }
}
