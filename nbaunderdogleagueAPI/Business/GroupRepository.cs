using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IGroupRepository
    {
        List<GroupStandings> GetGroupStandings(string groupId);
        GroupEntity CreateGroup(string name, string ownerEmail);
        string JoinGroup(JoinGroupRequest joinGroupRequest);
        string LeaveGroup(LeaveGroupRequest leaveGroupRequest);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year, bool includeUser, string email);
        List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year);
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
        public string JoinGroup(JoinGroupRequest joinGroupRequest)
        {
            return _groupDataAccess.JoinGroup(joinGroupRequest);
        }
        public string LeaveGroup(LeaveGroupRequest leaveGroupRequest)
        {
            return _groupDataAccess.LeaveGroup(leaveGroupRequest);
        }
        public GroupEntity GetGroup(string groupId)
        {
            return _groupDataAccess.GetGroup(groupId);
        }
        public List<GroupEntity> GetAllGroupsByYear(int year, bool includeUser, string email)
        {
            return _groupDataAccess.GetAllGroupsByYear(year, includeUser, email);
        }
        public List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year)
        {
            return _groupDataAccess.GetAllGroupsUserIsInByYear(email, year);
        }
    }
}
