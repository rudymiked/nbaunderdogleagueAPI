﻿using Microsoft.AspNetCore.Mvc.Testing;
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
        public void TeamStatsListFromJSON()
        {
            if (_teamService != null) {
                List<TeamStats> teamStats = _teamService.TeamStatsListFromJSON();

                Assert.AreNotEqual(0, teamStats.Count);

                Dictionary<string, TeamStats> teamStatsDict = _teamService.TeamStatsDictionaryFromJSON();

                Assert.AreNotEqual(0, teamStatsDict.Count);
            } else {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public void GetTeamStatsTestV1()
        //{
        //    if (_teamService != null) {
        //        List<TeamStats> teamStats = _teamService.TeamStatsList(1);

        //        Assert.AreNotEqual(0, teamStats.Count);

        //        Dictionary<string, TeamStats> teamStatsDict = _teamService.TeamStatsDictionary(1);

        //        Assert.AreNotEqual(0, teamStatsDict.Count);
        //    } else {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public void TeamStatsListFromStorage()
        {
            if (_teamService != null) {
                List<TeamStats> teamStats = _teamService.TeamStatsListFromStorage();

                Assert.AreNotEqual(0, teamStats.Count);

                Dictionary<string, TeamStats> teamStatsDict = _teamService.TeamStatsDictionaryFromStorage();

                Assert.AreNotEqual(0, teamStatsDict.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetTeamsEntity()
        {
            if (_teamService != null) {
                List<TeamEntity> teams = _teamService.GetTeams();

                Assert.AreNotEqual(0, teams.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateTeamStatsManually()
        {
            if (_teamService != null) {
                List<TeamStats> teamStats = _teamService.UpdateTeamStatsManually();

                Assert.AreNotEqual(0, teamStats.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateTeamPlayoffWins()
        {
            if (_teamService != null) {

                TeamStats teamStats = new();

                string result = _teamService.UpdateTeamPlayoffWins(teamStats);

                Assert.AreEqual(AppConstants.Success, result);
            } else {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public void AddTeams()
        //{
        //    if (_teamService != null) {
        //        List<TeamEntity> teamsEntities = new();
        //        StreamReader r = new StreamReader(@"C:\Users\rudym\source\repos\nbaunderdogleagueAPI\nbaunderdogleagueAPI\Data\NBA2023ODDS.json");

        //        string jsonString = r.ReadToEnd();

        //        List<ImportTeam> data = JsonConvert.DeserializeObject<List<ImportTeam>>(jsonString);

        //        for(int i = 0; i < data.Count; i++) {
        //            teamsEntities.Add(new TeamEntity() {
        //                PartitionKey = data[i].PartitionKey,
        //                RowKey = data[i].RowKey,
        //                ID = 0,
        //                City = data[i].City,
        //                Name = data[i].Name,
        //                ProjectedLoss = data[i].ProjectedLoss,
        //                ProjectedWin = data[i].ProjectedWin,
        //                Timestamp = data[i].Timestamp,
        //                ETag = ETag.All
        //            });
        //        }

        //        List<TeamEntity> teams = _teamService.AddTeams(teamsEntities);

        //        Assert.AreNotEqual(0, teams.Count);
        //    } else {
        //        Assert.Fail();
        //    }
        //}
    }
}
