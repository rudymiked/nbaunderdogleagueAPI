using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserLogic
    {
        List<User> GetUsers();
    }

    public class UserBusinessLogic : IUserLogic
    {
        public List<User> GetUsers()
        {
            UserDataAccess userDataAccess = new();

            return userDataAccess.GetUserData();
        }
    }
}
