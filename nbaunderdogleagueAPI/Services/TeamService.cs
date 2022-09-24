using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<Team> GetTeams();
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public List<Team> GetTeams()
        {
            return _teamRepository.GetTeams();
        }
    }
}
