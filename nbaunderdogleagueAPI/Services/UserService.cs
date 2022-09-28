using nbaunderdogleagueAPI.Business;

namespace nbaunderdogleagueAPI.Services
{
    public interface IUserService
    {
        List<UserEntity> GetUsers(string leagueId);
        User AddUser(User user);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserEntity> GetUsers(string leagueId)
        {
            return _userRepository.GetUsers(leagueId);
        }

        public User AddUser(User user)
        {
            return _userRepository.AddUser(user);
        }
    }
}
