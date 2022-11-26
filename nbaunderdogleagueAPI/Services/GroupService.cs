using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IGroupService
    {
        List<GroupStandings> GetGroupStandings(string groupId, int version);
        GroupEntity CreateGroup(string name, string ownerEmail);
        string JoinGroup(JoinGroupRequest joinGroupRequest);
        string LeaveGroup(LeaveGroupRequest leaveGroupRequest);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year, bool includeUser, string email);
        List<GroupEntity> GetAllGroupsUserIsInByYear(string user, int year);
        List<GroupEntity> GetAllGroups();
    }
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public List<GroupStandings> GetGroupStandings(string groupId, int version)
        {
            return _groupRepository.GetGroupStandings(groupId, version);
        }
        public GroupEntity CreateGroup(string name, string ownerEmail)
        {
            return _groupRepository.CreateGroup(name, ownerEmail);
        }
        public string JoinGroup(JoinGroupRequest joinGroupRequest)
        {
            return _groupRepository.JoinGroup(joinGroupRequest);
        }
        public string LeaveGroup(LeaveGroupRequest leaveGroupRequest)
        {
            return _groupRepository.LeaveGroup(leaveGroupRequest);
        }
        public GroupEntity GetGroup(string groupId)
        {
            return _groupRepository.GetGroup(groupId);
        }
        public List<GroupEntity> GetAllGroupsByYear(int year, bool includeUser, string email)
        {
            return _groupRepository.GetAllGroupsByYear(year, includeUser, email);
        }
        public List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year)
        {
            return _groupRepository.GetAllGroupsUserIsInByYear(email, year);
        }
        public List<GroupEntity> GetAllGroups()
        {
            return _groupRepository.GetAllGroups();
        }
    }
}
