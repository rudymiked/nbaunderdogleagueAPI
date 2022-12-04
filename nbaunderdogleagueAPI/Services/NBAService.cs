using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface INBAService
    {
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
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
    }
}
