using Azure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class TeamTests
    {
        private ITeamService _teamService;
        private AppConfig _appConfig;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _appConfig = application.Services.GetService<IOptions<AppConfig>>().Value;
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

        [TestMethod]
        public void AddTeamsTest()
        {
            if (_teamService != null) {
                List<TeamsEntity> teamsEntities = new() { new TeamsEntity() {
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = "Team",
                    Name = "Sun",
                    City = "Pheonix",
                    ProjectedWin = 80,
                    ProjectedLoss = 2,
                    ETag = ETag.All,
                    Timestamp = DateTime.UtcNow
                },
                new TeamsEntity() {
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = "Team",
                    Name = "Clippers",
                    City = "Los Angeles",
                    ProjectedWin = 40,
                    ProjectedLoss = 42,
                    ETag = ETag.All,
                    Timestamp = DateTime.UtcNow
                }
            };

                List<TeamsEntity> teams = _teamService.AddTeams(teamsEntities);

                Assert.AreNotEqual(0, teams.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
