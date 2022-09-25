using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<Team> GetTeams();
        List<TeamsEntity> GetTeamsEntity();
        List<TeamsEntity> AddTeams(List<TeamsEntity> teamsEntities);
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

        public List<TeamsEntity> GetTeamsEntity()
        {
            return _teamRepository.GetTeamsEntity();
        }

        public List<TeamsEntity> AddTeams(List<TeamsEntity> teamsEntities)
        {
            return _teamRepository.AddTeams(teamsEntities);
        }
    }
}
