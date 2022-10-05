using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<CurrentNBAStanding> GetCurrentNBAStandingsList();
        Dictionary<string, CurrentNBAStanding> GetCurrentNBAStandingsDictionary();
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public List<TeamEntity> GetTeams()
        {
            return _teamRepository.GetTeams();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamRepository.AddTeams(teamsEntities);
        }

        public List<CurrentNBAStanding> GetCurrentNBAStandingsList()
        {
            return _teamRepository.GetCurrentNBAStandingsList();
        }

        public Dictionary<string, CurrentNBAStanding> GetCurrentNBAStandingsDictionary()
        {
            return _teamRepository.GetCurrentNBAStandingsDictionary();
        }
    }
}
