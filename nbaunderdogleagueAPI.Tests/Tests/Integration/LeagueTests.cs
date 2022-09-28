using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class LeagueTests
    {
        private ILeagueService _leagueService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _leagueService = application.Services.GetService<ILeagueService>();
        }

        [TestMethod]
        public void GetLeagueStandingsTest()
        {
            if (_leagueService != null) {
                List<LeagueStandings> standings = _leagueService.GetLeagueStandings();

                Assert.AreNotEqual(0, standings.Count);
            } else {
                Assert.Fail();
            }
        }        

        [TestMethod]
        public void DraftTeamTest()
        {
            if (_leagueService != null) {
                User userDrafted = new() {
                    Email = "rudymiked@gmail.com",
                    Team = "76ers"
                };

                User user = _leagueService.DraftTeam(userDrafted);

                Assert.AreNotEqual(string.Empty, user.Team);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SetupDraftTest()
        {
            if (_leagueService != null) {
                List<Draft> draftResults= _leagueService.SetupDraft();

                Assert.AreNotEqual(string.Empty, draftResults.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
