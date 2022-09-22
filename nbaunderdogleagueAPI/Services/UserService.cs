using nbaunderdogleagueAPI.Business;

namespace nbaunderdogleagueAPI.Services
{
    public interface IUserService
    {
        List<User> GetUsers();
    }
    public class UserService : IUserService
    {
        private readonly IUserBusinessLogic _userBusinessLogic;
        public UserService()
        {
            _userBusinessLogic = new UserBusinessLogic();
        }
        public List<User> GetUsers()
        {
            return _userBusinessLogic.GetUsers();
        }
    }
}
