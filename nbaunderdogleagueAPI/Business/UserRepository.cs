﻿using nbaunderdogleagueAPI.DataAccess;

namespace nbaunderdogleagueAPI.Business
{
    public interface IUserRepository
    {
        List<UserEntity> GetUsers(string groupId);
        User AddUser(User user);
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
        public User AddUser(User user)
        {
            return _userDataAccess.AddUser(user);
        }
    }
}
