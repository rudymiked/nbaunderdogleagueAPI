using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface INBARepository
    {
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
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
    }
}
