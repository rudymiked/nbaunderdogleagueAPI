using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface INBARepository
    {
        List<TeamStats> UpdateTeamStatsFromRapidAPI(string Year = "");
        List<NBAGameEntity> UpdateScoreboardFromRapidAPI();
        List<Scoreboard> NBAScoreboard(string groupId = null);
        List<PlayoffData> UpdatePlayoffData();
        Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> GetPlayoffData(string season);
    }
    public class NBARepository : INBARepository
    {
        private readonly INBADataAccess _nbaDataAccess;
        public NBARepository(INBADataAccess nbaDataAccess)
        {
            _nbaDataAccess = nbaDataAccess;
        }
        public List<TeamStats> UpdateTeamStatsFromRapidAPI(string Year = "")
        {
            return _nbaDataAccess.UpdateTeamStatsFromRapidAPI(Year);
        }
        public List<NBAGameEntity> UpdateScoreboardFromRapidAPI()
        {
            return _nbaDataAccess.UpdateScoreboardFromRapidAPI();
        }
        public List<Scoreboard> NBAScoreboard(string groupId = null)
        {
            return _nbaDataAccess.NBAScoreboard(groupId);
        }        
        public List<PlayoffData> UpdatePlayoffData()
        {
            return _nbaDataAccess.UpdatePlayoffData();
        }        
        public Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> GetPlayoffData(string season)
        {
            return _nbaDataAccess.GetPlayoffData(season);
        }
    }
}
