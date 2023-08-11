using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface INBARepository
    {
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
        List<NBAGameEntity> UpdateGamesFromRapidAPI();
        List<Scoreboard> NBAScoreboard(string groupId);
    }
    public class NBARepository : INBARepository
    {
        private readonly INBADataAccess _nbaDataAccess;
        public NBARepository(INBADataAccess nbaDataAccess)
        {
            _nbaDataAccess = nbaDataAccess;
        }
        public List<TeamStats> UpdateTeamStatsFromRapidAPI()
        {
            return _nbaDataAccess.UpdateTeamStatsFromRapidAPI();
        }
        public List<NBAGameEntity> UpdateGamesFromRapidAPI()
        {
            return _nbaDataAccess.UpdateGamesFromRapidAPI();
        }
        public List<Scoreboard> NBAScoreboard(string groupId)
        {
            return _nbaDataAccess.NBAScoreboard(groupId);
        }
    }
}
