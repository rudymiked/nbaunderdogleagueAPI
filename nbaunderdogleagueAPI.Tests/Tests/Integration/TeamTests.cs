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
        public void GetStandingsTest()
        {
            if (_teamService != null) {
                List<Standings> users = _teamService.GetStandings();

                Assert.AreNotEqual(0, users.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetTeamsEntityTest()
        {
            if (_teamService != null) {
                List<TeamEntity> teams = _teamService.GetTeamsEntity();

                Assert.AreNotEqual(0, teams.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddTeamsTest()
        {
            if (_teamService != null) {
                List<TeamEntity> teamsEntities = new() { new TeamEntity() {
                    PartitionKey = "Team",
                    RowKey = "Suns",
                    Name = "Suns",
                    City = "Pheonix",
                    ProjectedWin = 64,
                    ProjectedLoss = 18,
                    ETag = ETag.All,
                    Timestamp = DateTime.UtcNow
                },
                new TeamEntity() {
                    PartitionKey = "Team",
                    RowKey = "Clippers",
                    Name = "Clippers",
                    City = "Los Angeles",
                    ProjectedWin = 40,
                    ProjectedLoss = 42,
                    ETag = ETag.All,
                    Timestamp = DateTime.UtcNow
                }
                ,
                new TeamEntity() {
                    PartitionKey = "Team",
                    RowKey = "Lakers",
                    Name = "Lakers",
                    City = "Los Angeles",
                    ProjectedWin = 20,
                    ProjectedLoss = 62,
                    ETag = ETag.All,
                    Timestamp = DateTime.UtcNow
                }
            };

                List<TeamEntity> teams = _teamService.AddTeams(teamsEntities);

                Assert.AreNotEqual(0, teams.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
