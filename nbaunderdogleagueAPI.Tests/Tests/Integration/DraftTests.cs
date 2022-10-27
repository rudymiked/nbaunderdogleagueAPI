﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Text.RegularExpressions;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class DraftTests
    {
        private IDraftService _draftService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _draftService = application.Services.GetService<IDraftService>();
        }    

        [TestMethod]
        public void DraftTeamTest()
        {
            if (_draftService != null) {
                User userDrafted = new() {
                    Email = TestConstants.Email,
                    Team = "Jazz",
                    Group = TestConstants.PostGroupId.ToString(),
                    Username = "",
                };

                string draftResult = _draftService.DraftTeam(userDrafted);

                Assert.AreNotEqual(string.Empty, draftResult);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SetupDraftTest()
        {
            if (_draftService != null) {

                DateTime draftStartDate = new DateTime();

                SetupDraftRequest setupDraftRequest = new() {
                    GroupId = TestConstants.PostGroupId.ToString(),
                    Email = TestConstants.Email,
                    ClearTeams = true,
                    DraftStartDateTime = draftStartDate,
                    DraftWindow = 5
                };

                List<DraftEntity> draftResults = _draftService.SetupDraft(setupDraftRequest);

                Assert.AreNotEqual(0, draftResults.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraftedTeamsTest()
        {
            if (_draftService != null) {
                List<UserEntity> draftedTeams = _draftService.DraftedTeams(TestConstants.GetGroupId.ToString());

                Assert.AreNotEqual(null, draftedTeams);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetDraftTest()
        {
            if (_draftService != null) {
                List<DraftEntity> draftResults = _draftService.GetDraft(TestConstants.GetGroupId.ToString());

                Assert.AreNotEqual(null, draftResults);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAvailableTeamsToDraft()
        {
            if (_draftService != null) {
                List<TeamEntity> availableTeams = _draftService.GetAvailableTeamsToDraft(TestConstants.GetGroupId.ToString());

                Assert.AreNotEqual(null, availableTeams);
            } else {
                Assert.Fail();
            }
        }
    }
}
