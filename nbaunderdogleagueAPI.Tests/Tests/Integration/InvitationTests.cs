using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class InvitationTests
    {
        private IInvitationService _invitationService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _invitationService = application.Services.GetService<IInvitationService>();
        }

        [TestMethod]
        public void GroupInvitationTest()
        {
            if (_invitationService != null) {
                Guid inviteId = Guid.NewGuid();

                GroupInvitation groupInvitation = new() {
                    InviteId = inviteId,
                    GroupId = TestConstants.TestGroupId,
                    Email = TestConstants.Email
                };

                GroupInvitationEntity sendGroup = _invitationService.SendGroupInvitation(groupInvitation);

                Assert.AreNotEqual(true, string.IsNullOrEmpty(sendGroup.PartitionKey));

                string acceptGroup = _invitationService.AcceptGroupInvitation(groupInvitation);

                Assert.AreEqual(AppConstants.Success, acceptGroup);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetGroupInvitationsTest()
        {
            if (_invitationService != null) {
                try {
                    List<GroupInvitationEntity> groupInvitations = _invitationService.GetGroupInvitations(TestConstants.TestGroupId);

                    Assert.IsTrue(true);
                } catch (Exception ex) {
                    Assert.Fail();
                }
            } else {
                Assert.Fail();
            }
        }
    }
}
