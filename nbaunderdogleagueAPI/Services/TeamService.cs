using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<Standings> GetStandings();
        List<TeamEntity> GetTeamsEntity();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public List<Standings> GetStandings()
        {
            return _teamRepository.GetStandings();
        }

        public List<TeamEntity> GetTeamsEntity()
        {
            return _teamRepository.GetTeamsEntity();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamRepository.AddTeams(teamsEntities);
        }
    }
}
