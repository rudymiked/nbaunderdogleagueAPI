using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class NBATests
    {
        private INBAService _nbaService;
        private IRapidAPIHelper _rapidAPIHelper;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _nbaService = application.Services.GetService<INBAService>();
            _rapidAPIHelper = application.Services.GetService<IRapidAPIHelper>();
        }

        [TestMethod]
        public void UpdateTeamStatsFromRapidAPI()
        {
            if (_nbaService != null && _rapidAPIHelper != null) {
                List<TeamStats> teamStats = _nbaService.UpdateTeamStatsFromRapidAPI();

                if (_rapidAPIHelper.IsRapidAPIAvailable()) {
                    Assert.AreNotEqual(0, teamStats.Count);
                } else {
                    Assert.AreEqual(0, teamStats.Count);
                }
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateGamesFromRapidAPI()
        {
            if (_nbaService != null && _rapidAPIHelper != null) {
                List<NBAGameEntity> gameData = _nbaService.UpdateGamesFromRapidAPI();

                if (_rapidAPIHelper.IsRapidAPIAvailable()) {
                    Assert.AreNotEqual(0, gameData.Count);
                } else {
                    Assert.AreEqual(0, gameData.Count);
                }
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NBAScoreboard()
        {
            if (_nbaService != null) {
                List<Scoreboard> scoreboard = _nbaService.NBAScoreboard();

                Assert.AreNotEqual(0, scoreboard.Count);
            } else {
                Assert.Fail();
            }   
        }

        //[TestMethod]
        //public void SetRapidAPITimeout()
        //{
        //    if (_nbaService != null) {
        //        DateTimeOffset now = DateTimeOffset.UtcNow;
        //        bool setTimeoutResult = _rapidAPIHelper.SetRapidAPITimeout(now.AddDays(1));

        //        Assert.AreEqual(true, setTimeoutResult);
        //    } else {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public void IsRapidAPIAvailable()
        {
            if (_nbaService != null) {
                try {
                    bool rapidAPIAvailable = _rapidAPIHelper.IsRapidAPIAvailable();

                    Assert.IsTrue(true);
                } catch (Exception) {
                    Assert.Fail();
                }
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdatePlayoffDataTest()
        {
            if (_nbaService != null) {
                try {
                    List<TeamStats> updatedData = _nbaService.UpdatePlayoffData();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Assert.Fail();
                }
            } else {
                Assert.Fail();
            }
        }
    }
}

