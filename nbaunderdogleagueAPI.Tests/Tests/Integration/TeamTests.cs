using Azure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using Newtonsoft.Json;

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
        public void GetCurrentNBAStandingsTest()
        {
            if (_teamService != null) {
                List<CurrentNBAStanding> currentNBAStandings = _teamService.GetCurrentNBAStandingsList();

                Assert.AreNotEqual(0, currentNBAStandings.Count);

                Dictionary<string, CurrentNBAStanding> currentNBAStandingsDict = _teamService.GetCurrentNBAStandingsDictionary();

                Assert.AreNotEqual(0, currentNBAStandingsDict.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetTeamsEntityTest()
        {
            if (_teamService != null) {
                List<TeamEntity> teams = _teamService.GetTeams();

                Assert.AreNotEqual(0, teams.Count);
            } else {
                Assert.Fail();
            }
        }

        // do not use, only for testing purposes
        //[TestMethod]
        //public void AddTeamsTest()
        //{
        //    if (_teamService != null) {
        //        List<TeamEntity> teamsEntities = new();
        //        StreamReader r = new StreamReader(@"C:\Users\rudym\source\repos\nbaunderdogleagueAPI\nbaunderdogleagueAPI\Data\NBA2023ODDS.json");

        //        string jsonString = r.ReadToEnd();

        //        var data = JsonConvert.DeserializeObject<TeamEntity[]>(jsonString);

        //        List<TeamEntity> teams = _teamService.AddTeams(teamsEntities);

        //        Assert.AreNotEqual(0, teams.Count);
        //    } else {
        //        Assert.Fail();
        //    }
        //}
    }
}
