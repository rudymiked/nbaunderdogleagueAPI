using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class DraftTests
    {
        private IDraftService _draftService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _draftService = application.Services.GetService<IDraftService>();
        }    

        [TestMethod]
        public void DraftTeamTest()
        {
            if (_draftService != null) {
                User userDrafted = new() {
                    Email = "rudymiked@gmail.com",
                    Team = "76ers",
                    Group = TestConstants.GroupId
                };

                Dictionary<User, string> user = _draftService.DraftTeam(userDrafted);

                Assert.AreNotEqual(string.Empty, user.Keys.First().Team);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SetupDraftTest()
        {
            if (_draftService != null) {
                List<DraftEntity> draftResults = _draftService.SetupDraft("09fa8ea1-1284-4ad8-b6d1-6f1377fdbac7");

                Assert.AreNotEqual(string.Empty, draftResults.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraftedTeamsTest()
        {
            if (_draftService != null) {
                List<UserEntity> draftedTeams = _draftService.DraftedTeams(TestConstants.GroupId.ToString());

                Assert.AreNotEqual(null, draftedTeams);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraftTest()
        {
            if (_draftService != null) {
                List<DraftEntity> draftResults = _draftService.GetDraft(TestConstants.GroupId.ToString());

                Assert.AreNotEqual(null, draftResults);
            } else {
                Assert.Fail();
            }
        }
    }
}
