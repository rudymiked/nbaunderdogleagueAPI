using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IGroupRepository
    {
        List<GroupStandings> GetGroupStandings(string groupId, int version);
        GroupEntity CreateGroup(string name, string ownerEmail);
        string JoinGroup(JoinGroupRequest joinGroupRequest);
        string LeaveGroup(LeaveGroupRequest leaveGroupRequest);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year);
        List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year);
        List<GroupEntity> GetAllGroups();
    }

    public class GroupRepository : IGroupRepository
    {
        private readonly IGroupDataAccess _groupDataAccess;
        public GroupRepository(IGroupDataAccess groupDataAccess)
        {
            _groupDataAccess = groupDataAccess;
        }
        public List<GroupStandings> GetGroupStandings(string groupId, int version)
        {
            return _groupDataAccess.GetGroupStandings(groupId, version);
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
        public List<GroupEntity> GetAllGroupsByYear(int year)
        {
            return _groupDataAccess.GetAllGroupsByYear(year);
        }
        public List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year)
        {
            return _groupDataAccess.GetAllGroupsUserIsInByYear(email, year);
        }
        public List<GroupEntity> GetAllGroups()
        {
            return _groupDataAccess.GetAllGroups();
        }
    }
}
