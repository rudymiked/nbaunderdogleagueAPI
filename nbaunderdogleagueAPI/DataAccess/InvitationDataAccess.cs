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
        string GenerateGroupJoinLink(GroupInvitation groupInvitation);
        string JoinRequest(GroupInvitation groupInvitation);
    }

    public class InvitationDataAccess : IInvitationDataAccess
    {
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly IEmailService _emailService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        public InvitationDataAccess(ITableStorageHelper tableStorageHelper, IEmailService emailService, IGroupService groupService, IUserService userService)
        {
            _tableStorageHelper = tableStorageHelper;
            _emailService = emailService;
            _groupService = groupService;
            _userService = userService;
        }

        private GroupInvitationEntity CreateNewInvitation(GroupInvitation groupInvitation, SendGrid.Response emailResponse = null)
        {
            Guid inviteId = Guid.NewGuid();

            GroupInvitationEntity groupInvitationEntity = new() {
                PartitionKey = groupInvitation.GroupId.ToString(),
                RowKey = inviteId.ToString(),
                InviteId = inviteId,
                GroupId = groupInvitation.GroupId,
                Email = groupInvitation.Email,
                Timestamp = DateTimeOffset.UtcNow,
                Approved = false,
                Declined = false,
                Notes = emailResponse == null ? "Link" : "Email Status: " + emailResponse.StatusCode.ToString(),
                Expiration = DateTimeOffset.UtcNow.AddDays(7),
                ETag = ETag.All
            };

            Response tableResponse = _tableStorageHelper.UpsertEntity(groupInvitationEntity, AppConstants.GroupInvitationsTable).Result;

            return (tableResponse == null || tableResponse.IsError) ? new GroupInvitationEntity() : groupInvitationEntity;
        }

        public GroupInvitationEntity SendGroupInvitation(GroupInvitation groupInvitation)
        {
            // 1. Send Email
            // 2. Populate GroupInvitationsTable

            // 1. Send Email
            SendGrid.Response emailResponse = _emailService.InviteUserToGroup(groupInvitation.Email, groupInvitation.GroupId.ToString()).Result;

            // 2. Populate GroupInvitationsTable and return result

            return CreateNewInvitation(groupInvitation, emailResponse);
        }

        public List<GroupInvitationEntity> GetGroupInvitations(Guid groupId)
        {
            string filter = TableClient.CreateQueryFilter<GroupInvitationEntity>((group) => group.GroupId == groupId);

            var response = _tableStorageHelper.QueryEntities<GroupInvitationEntity>(AppConstants.GroupInvitationsTable, filter).Result;

            return response.Any() ? response.ToList() : new List<GroupInvitationEntity>();
        }

        public string AcceptGroupInvitation(GroupInvitation groupInvitation)
        {
            // filter ensures that invitation exists and has not been approved, declined, or expired.
            string filter = TableClient.CreateQueryFilter<GroupInvitationEntity>((group) =>
                                group.GroupId == groupInvitation.GroupId &&
                                group.InviteId == groupInvitation.InviteId &&
                                group.Email == groupInvitation.Email &&
                                group.Approved == false &&
                                group.Expiration > DateTimeOffset.UtcNow);

            var queryResponse = _tableStorageHelper.QueryEntities<GroupInvitationEntity>(AppConstants.GroupInvitationsTable, filter).Result;

            if (queryResponse.Any()) {
                List<GroupInvitationEntity> groupInvitationEntities = queryResponse.ToList();

                if (groupInvitationEntities.Count > 1) {
                    // something is wrong, there are more than one identical invites
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

                groupInvitationEntity.Approved = true;
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

                // XXX perform individual validations to see why we couldn't accept the invitation (expired, used, etc.)
                return "Could not update invitation id: " + groupInvitation.InviteId;
            } else {
                return "There are no current group invitations";
            }
        }

        public string GenerateGroupJoinLink(GroupInvitation groupInvitation)
        {
            // admin or users in group can create this link to send out to potential group members
            // users will all click the same link and be brought to UI
            Uri url = new Uri(AppConstants.UIUrl + "/join/")
                      .AddQuery("groupId", groupInvitation.GroupId.ToString());

            return url.ToString();
        }

        public string JoinRequest(GroupInvitation groupInvitation)
        {
            // a potential user will click an UI endpoint
            // they are taken to a page in the site so they can sign in and provide their email
            // and added to a list of potential new group members
            // an admin can choose to add them to their group

            // potential validations

            // 1. email address needs to be valid <- done by UI
            // 2. group ID needs to be present and this year
            // 3. user isn't already in group

            // *. is group private or public? XXX not an option yet, but maybe it should be?

            // 1. handled by UI

            // 2.
            GroupEntity group = _groupService.GetGroup(groupInvitation.GroupId.ToString());

            if (group.Year != AppConstants.CurrentNBASeasonYear || group == null) {
                return AppConstants.GroupNotFound + groupInvitation.GroupId.ToString();
            }

            // 3.
            List<UserEntity> usersInGroup = _userService.GetUsers(groupInvitation.GroupId.ToString());

            if (!usersInGroup.Where(user => user.Email == groupInvitation.Email).Any()) {
                return AppConstants.UserAlreadyInGroup + group.Name;
            }

            GroupInvitationEntity groupInvitationEntity = CreateNewInvitation(groupInvitation);

            return groupInvitationEntity == null ? AppConstants.SomethingWentWrong : AppConstants.Success;
        }

        public string RejectGroupInvitation(GroupInvitation groupInvitation)
        {
            return string.Empty;
        }
    }
}
