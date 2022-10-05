using nbaunderdogleagueAPI.Business;

namespace nbaunderdogleagueAPI.Services
{
    public interface IUserService
    {
        List<UserEntity> GetUsers(string groupId);
        User UpsertUser(User user);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserEntity> GetUsers(string groupId)
        {
            return _userRepository.GetUsers(groupId);
        }

        public User UpsertUser(User user)
        {
            return _userRepository.UpsertUser(user);
        }
    }
}
