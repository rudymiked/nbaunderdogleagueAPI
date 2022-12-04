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
        public void UpdateTeamStatsFromRapidAPI()
        {
            if (_nbaService != null) {
                List<TeamStats> teamStats = _nbaService.UpdateTeamStatsFromRapidAPI();

                Assert.AreNotEqual(0, teamStats.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
