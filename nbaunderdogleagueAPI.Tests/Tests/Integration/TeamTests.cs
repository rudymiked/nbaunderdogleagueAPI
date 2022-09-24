using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class TeamTests
    {
        private ITeamService? _teamService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _teamService = application.Services.GetService<ITeamService>();
        }

        [TestMethod]
        public void GetTeamsTest()
        {
            if (_teamService != null) {
                List<Team> users = _teamService.GetTeams();

                Assert.AreNotEqual(0, users.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
