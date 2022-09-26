using nbaunderdogleagueAPI.Business;

namespace nbaunderdogleagueAPI.Services
{
    public interface IUserService
    {
        List<UserEntity> GetUsers();
        List<UserEntity> AddUsers(List<UserEntity> userEntities);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserEntity> GetUsers()
        {
            return _userRepository.GetUsers();
        }

        public List<UserEntity> AddUsers(List<UserEntity> userEntities)
        {
            return _userRepository.AddUsers(userEntities);
        }
    }
}
