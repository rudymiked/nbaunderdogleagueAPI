using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IGroupService
    {
        List<GroupStandings> GetGroupStandings(string groupId);
        GroupEntity CreateGroup(string name, string ownerEmail);
        string JoinGroup(string id, string email);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year);
        List<GroupEntity> GetAllGroupsUserIsInByYear(string user, int year);
    }
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public List<GroupStandings> GetGroupStandings(string groupId)
        {
            return _groupRepository.GetGroupStandings(groupId);
        }
        public GroupEntity CreateGroup(string name, string ownerEmail)
        {
            return _groupRepository.CreateGroup(name, ownerEmail);
        }
        public string JoinGroup(string id, string email)
        {
            return _groupRepository.JoinGroup(id, email);
        }
        public GroupEntity GetGroup(string groupId)
        {
            return _groupRepository.GetGroup(groupId);
        }
        public List<GroupEntity> GetAllGroupsByYear(int year)
        {
            return _groupRepository.GetAllGroupsByYear(year);
        }
        public List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year)
        {
            return _groupRepository.GetAllGroupsUserIsInByYear(email, year);
        }
    }
}
