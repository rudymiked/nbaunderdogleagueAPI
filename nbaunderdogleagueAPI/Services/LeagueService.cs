using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ILeagueService
    {
        List<LeagueStandings> GetLeagueStandings();
    }
    public class LeagueService : ILeagueService
    {
        private readonly ILeagueRepository _leagueRepository;
        public LeagueService(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }
        public List<LeagueStandings> GetLeagueStandings()
        {
            return _leagueRepository.GetLeagueStandings();
        }
    }
}
