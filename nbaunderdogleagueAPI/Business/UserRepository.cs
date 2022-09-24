using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserRepository
    {
        List<User> GetUsers();
    }

    public class UserRepository : IUserRepository
    {
        private readonly IUserDataAccess _userDataAccess;
        public UserRepository()
        {
            _userDataAccess = new UserDataAccess();
        }
        public List<User> GetUsers()
        {
            return _userDataAccess.GetUserData();
        }
    }
}
