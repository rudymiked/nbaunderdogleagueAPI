﻿using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserRepository
    {
        List<UserEntity> GetUsers(string groupId);
        User UpserUser(User user);
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
        public User UpserUser(User user)
        {
            return _userDataAccess.UpserUser(user);
        }
    }
}
