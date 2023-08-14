using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Models.PlayerStatistics;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class PlayerTests
    {
        private IPlayerService _playerService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => {
                    builder.ConfigureAppConfiguration((hostingContext, config) => {

                    });
                });

            _playerService = application.Services.GetService<IPlayerService>();
        }

        [TestMethod]
        public void UpdatePlayerStatsFromRapidAPI()
        {
            List<PlayerStatisticsEntity> content = _playerService.UpdatePlayerStatsFromRapidAPI(season: 2022);

            Assert.IsTrue(content != null);
        }          
        
        [TestMethod]
        public void GetPlayerStatistics()
        {
            List<PlayerStatisticsEntity> content = _playerService.GetPlayerStatistics();

            Assert.IsTrue(content != null);
        }        
        
        [TestMethod]
        public void GetPlayerStatsPerTeamFromRapidAPI()
        {
            PlayerStatistics.PlayerResponse response = _playerService.GetPlayerStatsPerTeamFromRapidAPI(1, 2022);

            Assert.IsTrue(response.Players != null);
        }
    }
}
