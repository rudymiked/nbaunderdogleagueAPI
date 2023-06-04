using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
        public void DraftTeam()
        {
            if (_draftService != null) {
                User userDrafted = new() {
                    Email = TestConstants.NBAEmail,
                    Team = "Jazz",
                    Group = TestConstants.PostGroupId_TEST.ToString(),
                    Username = "",
                };

                string draftResult = _draftService.DraftTeam(userDrafted);

                Assert.AreNotEqual(string.Empty, draftResult);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SetupDraft()
        {
            if (_draftService != null) {

                DateTime draftStartDate = DateTime.Now;

                SetupDraftRequest setupDraftRequest = new() {
                    GroupId = TestConstants.PostGroupId_TEST.ToString(),
                    Email = TestConstants.NBAEmail,
                    ClearTeams = true,
                    DraftStartDateTime = draftStartDate,
                    DraftWindow = 5
                };

                List<DraftEntity> draftResults = _draftService.SetupDraft(setupDraftRequest);

                Assert.AreNotEqual(0, draftResults.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraftedTeams()
        {
            if (_draftService != null) {
                List<UserEntity> draftedTeams = _draftService.DraftedTeams(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(null, draftedTeams);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraft()
        {
            if (_draftService != null) {
                List<DraftEntity> draftResults = _draftService.GetDraft(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(null, draftResults);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAvailableTeamsToDraft()
        {
            if (_draftService != null) {
                List<TeamEntity> availableTeams = _draftService.GetAvailableTeamsToDraft(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(null, availableTeams);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraftResults()
        {
            if (_draftService != null) {
                List<DraftResults> draftResults = _draftService.GetDraftResults(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(null, draftResults);
            } else {
                Assert.Fail();
            }
        }
    }
}
