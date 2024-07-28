using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface INBAService
    {
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
        List<NBAGameEntity> UpdateGamesFromRapidAPI();
        List<Scoreboard> NBAScoreboard(string groupId = null);
        List<TeamStats> UpdatePlayoffData();
    }
    public class NBAService : INBAService
    {
        private readonly INBARepository _nbaRespository;
        public NBAService(INBARepository nbaRespository)
        {
            _nbaRespository = nbaRespository;
        }
        public List<TeamStats> UpdateTeamStatsFromRapidAPI()
        {
            return _nbaRespository.UpdateTeamStatsFromRapidAPI();
        }
        public List<NBAGameEntity> UpdateGamesFromRapidAPI()
        {
            return _nbaRespository.UpdateGamesFromRapidAPI();
        }
        public List<Scoreboard> NBAScoreboard(string groupId = null)
        {
            return _nbaRespository.NBAScoreboard(groupId);
        }        
        public List<TeamStats> UpdatePlayoffData()
        {
            return _nbaRespository.UpdatePlayoffData();
        }
    }
}
