using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Models.NBAModels;

namespace nbaunderdogleagueAPI.Business
{
    public interface IGroupRepository
    {
        List<GroupStandings> GetGroupStandings(string groupId);
        GroupEntity CreateGroup(string name, string ownerEmail);
        string JoinGroup(string id, string email);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year);
        List<GroupEntity> GetAllGroupsUserIsInByYear(string user, int year);
    }

    public class GroupRepository : IGroupRepository
    {
        private readonly IGroupDataAccess _groupDataAccess;
        public GroupRepository(IGroupDataAccess groupDataAccess)
        {
            _groupDataAccess = groupDataAccess;
        }
        public List<GroupStandings> GetGroupStandings(string groupId)
        {
            return _groupDataAccess.GetGroupStandings(groupId);
        }
        public GroupEntity CreateGroup(string name, string ownerEmail)
        {
            return _groupDataAccess.CreateGroup(name, ownerEmail);
        }
        public string JoinGroup(string id, string email)
        {
            return _groupDataAccess.JoinGroup(id, email);
        }
        public GroupEntity GetGroup(string groupId)
        {
            return _groupDataAccess.GetGroup(groupId);
        }
        public List<GroupEntity> GetAllGroupsByYear(int year)
        {
            return _groupDataAccess.GetAllGroupsByYear(year);
        }
        public List<GroupEntity> GetAllGroupsUserIsInByYear(string user, int year)
        {
            return _groupDataAccess.GetAllGroupsUserIsInByYear(user, year);
        }
    }
}
