using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Tests.Integration
{
    [TestClass]
    public class NBATests
    {
        private INBAService _nbaService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _nbaService = application.Services.GetService<INBAService>();
        }

        [TestMethod]
        public void UpdateTeamStatsFromRapidAPITest()
        {
            if (_nbaService != null) {
                List<TeamStats> teamStats = _nbaService.UpdateTeamStatsFromRapidAPI();

                Assert.AreNotEqual(0, teamStats.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateGamesFromRapidAPITest()
        {
            if (_nbaService != null) {
                List<NBAGameEntity> gameData = _nbaService.UpdateGamesFromRapidAPI();

                Assert.AreNotEqual(0, gameData.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NBAScoreboardTest()
        {
            if (_nbaService != null) {
                List<NBAGameEntity> scoreboard = _nbaService.NBAScoreboard();

                Assert.AreNotEqual(0, scoreboard.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}

