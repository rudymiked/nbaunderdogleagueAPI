﻿using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IInvitationService
    {
        GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation);
        string AcceptGroupInvitation(GroupInvitation groupInvitation);
        List<GroupInvitationEntity> GetGroupInvitations(Guid groupId);
        string JoinRequest(GroupInvitation groupInvitation);
        string GenerateGroupJoinLink(GroupInvitation groupInvitation);
    }

    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        public InvitationService(IInvitationRepository invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }
        public GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation)
        {
            return _invitationRepository.SendGroupInvitation(groupInvitation);
        }
        public string AcceptGroupInvitation(GroupInvitation groupInvitation)
        {
            return _invitationRepository.AcceptGroupInvitation(groupInvitation);
        }
        public List<GroupInvitationEntity> GetGroupInvitations(Guid groupId)
        {
            return _invitationRepository.GetGroupInvitations(groupId);
        }
        public string JoinRequest(GroupInvitation groupInvitation)
        {
            return _invitationRepository.JoinRequest(groupInvitation);
        }
        public string GenerateGroupJoinLink(GroupInvitation groupInvitation)
        {
            return _invitationRepository.GenerateGroupJoinLink(groupInvitation);
        }
    }
}
