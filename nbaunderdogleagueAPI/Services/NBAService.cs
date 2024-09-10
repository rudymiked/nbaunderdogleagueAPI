using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface INBAService
    {
        List<TeamStats> UpdateTeamStatsFromRapidAPI(string Year = "");
        List<NBAGameEntity> UpdateScoreboardFromRapidAPI();
        List<Scoreboard> NBAScoreboard(string groupId = null);
        List<PlayoffData> UpdatePlayoffData();
        Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> GetPlayoffData(string season);
    }
    public class NBAService : INBAService
    {
        private readonly INBARepository _nbaRespository;
        public NBAService(INBARepository nbaRespository)
        {
            _nbaRespository = nbaRespository;
        }
        public List<TeamStats> UpdateTeamStatsFromRapidAPI(string Year = "")
        {
            return _nbaRespository.UpdateTeamStatsFromRapidAPI(Year);
        }
        public List<NBAGameEntity> UpdateScoreboardFromRapidAPI()
        {
            return _nbaRespository.UpdateScoreboardFromRapidAPI();
        }
        public List<Scoreboard> NBAScoreboard(string groupId = null)
        {
            return _nbaRespository.NBAScoreboard(groupId);
        }        
        public List<PlayoffData> UpdatePlayoffData()
        {
            return _nbaRespository.UpdatePlayoffData();
        }        
        public Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> GetPlayoffData(string season)
        {
            return _nbaRespository.GetPlayoffData(season);
        }
    }
}
