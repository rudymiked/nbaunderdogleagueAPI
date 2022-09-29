using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Models.NBAModels;

namespace nbaunderdogleagueAPI.Business
{
    public interface ILeagueRepository
    {
        List<LeagueStandings> GetLeagueStandings(string leagueId);
        LeagueInfo CreateLeague(string name, string ownerEmail);
        string JoinLeague(string id, string email);
        LeagueEntity GetLeague(string leagueId);
    }

    public class LeagueRepository : ILeagueRepository
    {
        private readonly ILeagueDataAccess _leagueDataAccess;
        public LeagueRepository(ILeagueDataAccess leagueDataAccess)
        {
            _leagueDataAccess = leagueDataAccess;
        }
        public List<LeagueStandings> GetLeagueStandings(string leagueId)
        {
            return _leagueDataAccess.GetLeagueStandings(leagueId);
        }
        public LeagueInfo CreateLeague(string name, string ownerEmail)
        {
            return _leagueDataAccess.CreateLeague(name, ownerEmail);
        }
        public string JoinLeague(string id, string email)
        {
            return _leagueDataAccess.JoinLeague(id, email);
        }
        public LeagueEntity GetLeague(string leagueId)
        {
            return _leagueDataAccess.GetLeague(leagueId);
        }
    }
}
