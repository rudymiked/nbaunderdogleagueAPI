using nbaunderdogleagueAPI.Business;

namespace nbaunderdogleagueAPI.Services
{
    public interface IUserService
    {
        List<User> GetUsers();
    }
    public class UserService : IUserService
    {
        public List<User> GetUsers()
        {
            UserBusinessLogic userLogic = new();
            return userLogic.GetUsers();
        }
    }
}
