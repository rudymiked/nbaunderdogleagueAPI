using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetTeams(string Year = "");
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> TeamStatsListFromStorage(string Year = "");
        List<TeamStats> TeamStatsListFromJSON();
        List<TeamStats> TeamStatsListFromNBAdotCom();
        List<TeamStats> UpdateTeamStatsManually();
        Dictionary<string, TeamStats> TeamStatsDictionaryFromStorage(string Year = "");
        Dictionary<string, TeamStats> TeamStatsDictionaryFromJSON();
        Dictionary<string, TeamStats> TeamStatsDictionaryFromNBAdotCom();
        string UpdateTeamPlayoffWins(TeamStats teamStats);
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly ITeamDataAccess _teamDataAccess;
        public TeamRepository(ITeamDataAccess teamDataAccess)
        {
            _teamDataAccess = teamDataAccess;
        }

        public List<TeamEntity> GetTeams(string Year = "")
        {
            return _teamDataAccess.GetTeams(Year);
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities)
        {
            return _teamDataAccess.AddTeams(teamsEntities);
        }

        public List<TeamStats> TeamStatsListFromStorage(string Year = "")
        {
            return _teamDataAccess.GetTeamStatsFromStorage(Year).Values.OrderByDescending(team => team.Wins).ToList();
        }

        public List<TeamStats> TeamStatsListFromJSON()
        {
            return _teamDataAccess.GetTeamStatsFromJSON().Result.Values.OrderByDescending(team => team.Wins).ToList();
        }

        public List<TeamStats> TeamStatsListFromNBAdotCom()
        {
            return _teamDataAccess.GetTeamStatsFromNBAdotCom().Values.OrderByDescending(team => team.Wins).ToList();
        }

        public Dictionary<string, TeamStats> TeamStatsDictionaryFromStorage(string Year = "")
        {
            return _teamDataAccess.GetTeamStatsFromStorage(Year);
        }        
        
        public Dictionary<string, TeamStats> TeamStatsDictionaryFromJSON()
        {
            return _teamDataAccess.GetTeamStatsFromJSON().Result;
        }  
        
        public Dictionary<string, TeamStats> TeamStatsDictionaryFromNBAdotCom()
        {
            return _teamDataAccess.GetTeamStatsFromNBAdotCom();
        }

        public List<TeamStats> UpdateTeamStatsManually()
        {
            return _teamDataAccess.UpdateTeamStatsManually();
        }

        public string UpdateTeamPlayoffWins(TeamStats teamStats)
        {
            return _teamDataAccess.UpdateTeamPlayoffWins(teamStats);
        }
    }
}
