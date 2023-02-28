using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IInvitationRepository
    {
        GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation);
        string AcceptGroupInvitation(GroupInvitation groupInvitation);
        List<GroupInvitationEntity> GetGroupInvitations(Guid groupId);
    }

    public class InvitationRepository : IInvitationRepository
    {
        private readonly IInvitationDataAccess _invitationDataAccess;
        public InvitationRepository(IInvitationDataAccess invitationDataAccess)
        {
            _invitationDataAccess = invitationDataAccess;
        }
        public GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation)
        {
            return _invitationDataAccess.SendGroupInvitation(groupInvitation);
        }
        public string AcceptGroupInvitation(GroupInvitation groupInvitation)
        {
            return _invitationDataAccess.AcceptGroupInvitation(groupInvitation);
        }
        public List<GroupInvitationEntity> GetGroupInvitations(Guid groupId)
        {
            return _invitationDataAccess.GetGroupInvitations(groupId);
        }
    }
}
