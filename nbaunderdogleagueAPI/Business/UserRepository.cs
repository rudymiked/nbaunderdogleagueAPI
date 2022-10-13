using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserRepository
    {
        List<UserEntity> GetUsers(string groupId);
        User UpsertUser(User user);
        User UpdateUser(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IUserDataAccess _userDataAccess;
        public UserRepository(IUserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }
        public List<UserEntity> GetUsers(string groupId)
        {
            return _userDataAccess.GetUsers(groupId);
        }
        public User UpsertUser(User user)
        {
            return _userDataAccess.UpsertUser(user);
        }
        public User UpdateUser(User user)
        {
            return _userDataAccess.UpdateUser(user);
        }
    }
}
