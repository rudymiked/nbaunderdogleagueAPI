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

                if (_nbaService.IsRapidAPIAvailable()) {
                    Assert.AreNotEqual(0, teamStats.Count);
                } else {
                    Assert.AreEqual(0, teamStats.Count);
                }
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateGamesFromRapidAPITest()
        {
            if (_nbaService != null) {
                List<NBAGameEntity> gameData = _nbaService.UpdateGamesFromRapidAPI();

                if (_nbaService.IsRapidAPIAvailable()) {
                    Assert.AreNotEqual(0, gameData.Count);
                } else {
                    Assert.AreEqual(0, gameData.Count);
                }
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NBAScoreboardTest()
        {
            if (_nbaService != null) {
                List<Scoreboard> scoreboard = _nbaService.NBAScoreboard(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(0, scoreboard.Count);
            } else {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public void SetRapidAPITimeoutTest()
        //{
        //    if (_nbaService != null) {
        //        DateTimeOffset now = DateTimeOffset.UtcNow;
        //        bool setTimeoutResult = _nbaService.SetRapidAPITimeout(now.AddDays(1));

        //        Assert.AreEqual(true, setTimeoutResult);
        //    } else {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public void IsRapidAPIAvailableTest()
        {
            if (_nbaService != null) {
                try {
                    bool rapidAPIAvailable = _nbaService.IsRapidAPIAvailable();

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

