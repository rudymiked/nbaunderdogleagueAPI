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
                // need to query for an actual league first

                List<LeagueStandings> standings = _leagueService.GetLeagueStandings("");

                Assert.AreNotEqual(0, standings.Count);
            } else {
                Assert.Fail();
            }
        }        

        [TestMethod]
        public void CreateLeague()
        {
            if (_leagueService != null) {
                LeagueInfo newLeague = _leagueService.CreateLeague("Black Lung", "rudymiked@gmail.com");

                Assert.AreNotEqual(string.Empty, newLeague.Id);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetLeague()
        {
            if (_leagueService != null) {
                // need to query for an existing league first
                LeagueEntity league = _leagueService.GetLeague("");

                Assert.AreNotEqual(string.Empty, league.Name);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void JoinLeague()
        {
            if (_leagueService != null) {
                // need to query for an existing league first
                // also need a user email
                string leagueResult = _leagueService.JoinLeague("", "");

                Assert.AreNotEqual(AppConstants.Success, leagueResult);
            } else {
                Assert.Fail();
            }
        }
    }
}
