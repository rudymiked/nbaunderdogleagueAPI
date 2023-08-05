using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IPlayerRepository
    {
    }
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IPlayerDataAccess _playerDataAccess;
        public PlayerRepository(IPlayerDataAccess playerDataAccess)
        {
            _playerDataAccess = playerDataAccess;
        }
    }
}
