using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ITeamService
    {
        List<TeamEntity> GetTeams(string Year = "");
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> TeamStatsListFromStorage(string Year = "");
        List<TeamStats> TeamStatsListFromJSON();
        List<TeamStats> TeamStatsListFromNBAdotCom();
        Dictionary<string, TeamStats> TeamStatsDictionaryFromStorage(string Year = "");
        Dictionary<string, TeamStats> TeamStatsDictionaryFromJSON();
        Dictionary<string, TeamStats> TeamStatsDictionaryFromNBAdotCom();
        List<TeamStats> UpdateTeamStatsManually();
        string UpdateTeamPlayoffWins(TeamStats teamStats);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public List<TeamEntity> GetTeams(string Year = "")
        {
            return _teamRepository.GetTeams(Year);
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamRepository.AddTeams(teamsEntities);
        }

        public List<TeamStats> TeamStatsListFromStorage(string Year = "")
        {
            return _teamRepository.TeamStatsListFromStorage(Year);
        }       

        public List<TeamStats> TeamStatsListFromJSON()
        {
            return _teamRepository.TeamStatsListFromJSON();
        }    
        
        public List<TeamStats> TeamStatsListFromNBAdotCom()
        {
            return _teamRepository.TeamStatsListFromNBAdotCom();
        }

        public Dictionary<string, TeamStats> TeamStatsDictionaryFromStorage(string Year = "")
        {
            return _teamRepository.TeamStatsDictionaryFromStorage(Year);
        }       
        
        public Dictionary<string, TeamStats> TeamStatsDictionaryFromJSON()
        {
            return _teamRepository.TeamStatsDictionaryFromJSON();
        }        
        
        public Dictionary<string, TeamStats> TeamStatsDictionaryFromNBAdotCom()
        {
            return _teamRepository.TeamStatsDictionaryFromNBAdotCom();
        }

        public List<TeamStats> UpdateTeamStatsManually()
        {
            return _teamRepository.UpdateTeamStatsManually();
        }

        public string UpdateTeamPlayoffWins(TeamStats teamStats)
        {
            return _teamRepository.UpdateTeamPlayoffWins(teamStats);
        }
    }
}
