using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserRepository
    {
        List<UserEntity> GetUsers();
        List<UserEntity> AddUsers(List<UserEntity> userEntities);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IUserDataAccess _userDataAccess;
        public UserRepository(IUserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }
        public List<UserEntity> GetUsers()
        {
            return _userDataAccess.GetUsers();
        }        
        public List<UserEntity> AddUsers(List<UserEntity> userEntities)
        {
            return _userDataAccess.AddUsers(userEntities);
        }
    }
}
