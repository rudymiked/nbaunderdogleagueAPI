using Azure;
using Azure.Data.Tables;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IInvitationDataAccess
    {
        GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation);
        string AcceptGroupInvitation(GroupInvitation groupInvitation);
        List<GroupInvitationEntity> GetGroupInvitations(Guid groupId);
    }

    public class InvitationDataAccess : IInvitationDataAccess
    {
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly IEmailService _emailService;
        private readonly IGroupService _groupService;
        public InvitationDataAccess(ITableStorageHelper tableStorageHelper, IEmailService emailService, IGroupService groupService)
        {
            _tableStorageHelper = tableStorageHelper;
            _emailService = emailService;
            _groupService = groupService;
        }
        public GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation)
        {
            // 1. Send Email
            // 2. Populate GroupInvitationsTable

            SendGrid.Response emailResponse = _emailService.InviteUserToGroup(groupInvitation.Email, groupInvitation.GroupId.ToString()).Result;

            GroupInvitationEntity groupInvitationEntity = new() {
                PartitionKey = groupInvitation.GroupId.ToString(),
                RowKey = groupInvitation.Email,
                InviteId = groupInvitation.InviteId == Guid.Empty ? Guid.NewGuid() : groupInvitation.InviteId,
                GroupId = groupInvitation.GroupId,
                Email = groupInvitation.Email,
                Timestamp = DateTimeOffset.UtcNow,
                Used = false,
                EmailSent = emailResponse.IsSuccessStatusCode,
                Expiration = DateTimeOffset.UtcNow.AddDays(7),
                ETag = ETag.All
            };

            Response tableResponse = _tableStorageHelper.UpsertEntity(groupInvitationEntity, AppConstants.GroupInvitationsTable).Result;

            return (tableResponse == null || tableResponse.IsError) ? new GroupInvitationEntity() : groupInvitationEntity;
        }

        public List<GroupInvitationEntity> GetGroupInvitations(Guid groupId)
        {
            string filter = TableClient.CreateQueryFilter<GroupInvitationEntity>((group) => group.GroupId == groupId);

            var response = _tableStorageHelper.QueryEntities<GroupInvitationEntity>(AppConstants.GroupInvitationsTable, filter).Result;

            return response.Any() ? response.ToList() : new List<GroupInvitationEntity>();
        }

        public string AcceptGroupInvitation(GroupInvitation groupInvitation)
        {
            // filter ensures that invitation exists and has not been used or expired.
            string filter = TableClient.CreateQueryFilter<GroupInvitationEntity>((group) =>
                                group.GroupId == groupInvitation.GroupId &&
                                group.InviteId == groupInvitation.InviteId &&
                                group.Email == groupInvitation.Email &&
                                group.Used == false &&
                                group.Expiration > DateTimeOffset.UtcNow);

            var queryResponse = _tableStorageHelper.QueryEntities<GroupInvitationEntity>(AppConstants.GroupInvitationsTable, filter).Result;

            if (queryResponse.Any()) {
                List<GroupInvitationEntity> groupInvitationEntities = queryResponse.ToList();

                if (groupInvitationEntities.Count > 1) {
                    // something is wrong, there are more than one idential invites
                    // XXX LOG THIS
                }

                // Add user to group
                JoinGroupRequest joinGroupRequest = new() {
                    GroupId = groupInvitation.GroupId.ToString(),
                    Email = groupInvitation.Email
                };

                string joinGroup = _groupService.JoinGroup(joinGroupRequest);

                if (joinGroup != AppConstants.Success) {
                    // something went wrong adding this user to the group
                    return "Could not add user " + groupInvitation.Email + " to group id: " + groupInvitation.GroupId;
                }

                // update invitation so it can't be used again
                GroupInvitationEntity groupInvitationEntity = groupInvitationEntities.FirstOrDefault();

                groupInvitationEntity.Used = true;
                groupInvitationEntity.Expiration = DateTimeOffset.UtcNow;

                Response updateResponse = _tableStorageHelper.UpdateEntity(groupInvitationEntity, AppConstants.GroupInvitationsTable).Result;

                if (updateResponse.IsError) {
                    return "Could not update invitation id: " + groupInvitation.InviteId;
                }

                return AppConstants.Success;
            }

            // did not find invitation, let's see why:

            var notFoundResponse = _tableStorageHelper.QueryEntities<GroupInvitationEntity>(AppConstants.GroupInvitationsTable, string.Empty).Result;

            if (notFoundResponse.Any()) {
                List<GroupInvitationEntity> notFoundEntities = notFoundResponse.ToList();

                return "Could not update invitation id: " + groupInvitation.InviteId;
            } else {
                return "There are no current group invitations";
            }

            return AppConstants.SomethingWentWrong;
        }
    }
}
