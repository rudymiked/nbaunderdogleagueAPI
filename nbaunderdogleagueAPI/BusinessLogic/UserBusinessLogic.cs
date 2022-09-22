using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserBusinessLogic
    {
        List<User> GetUsers();
    }

    public class UserBusinessLogic : IUserBusinessLogic
    {
        private readonly IUserDataAccess _userDataAccess;
        public UserBusinessLogic()
        {
            _userDataAccess = new UserDataAccess();
        }
        public List<User> GetUsers()
        {
            return _userDataAccess.GetUserData();
        }
    }
}
