using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ILeagueService
    {
        List<LeagueStandings> GetLeagueStandings();
        User DraftTeam(User user);
        List<Draft> SetupDraft(string id);
        League CreateLeague(string name, string ownerEmail);
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
        public User DraftTeam(User user)
        {
            return _leagueRepository.DraftTeam(user);
        }
        public List<Draft> SetupDraft(string id)
        {
            return _leagueRepository.SetupDraft(id);
        }        
        public League CreateLeague(string name, string ownerEmail)
        {
            return _leagueRepository.CreateLeague(name, ownerEmail);
        }
    }
}
