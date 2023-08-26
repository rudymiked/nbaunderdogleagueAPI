﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class GroupTests
    {
        private IGroupService _groupService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _groupService = application.Services.GetService<IGroupService>();
        }

        [TestMethod]
        public void GetGroupStandingsTestV0()
        {
            if (_groupService != null) {
                List<GroupStandings> standings = _groupService.GetGroupStandings(AppConstants.Group_2022.ToString(), 0);

                Assert.AreNotEqual(0, standings.Count);
            } else {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public void GetGroupStandingsTestV1()
        //{
        //    if (_groupService != null) {
        //        List<GroupStandings> standings = _groupService.GetGroupStandings(AppConstants.Group_2022.ToString(), 1);

        //        Assert.AreNotEqual(0, standings.Count);
        //    } else {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public void GetGroupStandingsTestV2()
        {
            if (_groupService != null) {
                List<GroupStandings> standings = _groupService.GetGroupStandings(AppConstants.Group_2022.ToString(), 2);

                Assert.AreNotEqual(0, standings.Count);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CreateGroup()
        {
            if (_groupService != null) {
                GroupEntity newGroup = _groupService.CreateGroup("Test Group " + DateTime.Now.ToShortDateString(), TestConstants.NBAEmail);

                Assert.AreNotEqual(Guid.Empty, newGroup.Id);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetGroup()
        {
            if (_groupService != null) {
                GroupEntity group = _groupService.GetGroup(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(string.Empty, group.Name);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAllGroups()
        {
            if (_groupService != null) {
                List<GroupEntity> groupEntities = _groupService.GetAllGroups();

                Assert.AreNotEqual(null, groupEntities);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAllGroupsByYear()
        {
            if (_groupService != null) {
                List<GroupEntity> groups = _groupService.GetAllGroupsByYear(AppConstants.CurrentNBASeasonYear);

                Assert.AreEqual(true, groups.Any());
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAllGroupsUserIsInByYear()
        {
            if (_groupService != null) {
                List<GroupEntity> groups = _groupService.GetAllGroupsUserIsInByYear(TestConstants.Email, AppConstants.CurrentNBASeasonYear);

                Assert.AreEqual(true, groups.Any());
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void JoinGroup()
        {
            if (_groupService != null) {

                JoinGroupRequest joinGroupRequest = new() {
                    GroupId = TestConstants.PostGroupId_TEST.ToString(),
                    Email = TestConstants.Email
                };

                string groupResult = _groupService.JoinGroup(joinGroupRequest);

                Assert.AreEqual(AppConstants.Success, groupResult);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void LeaveGroup()
        {
            if (_groupService != null) {

                LeaveGroupRequest leaveGroupRequest = new() {
                    GroupId = TestConstants.PostGroupId_TEST.ToString(),
                    Email = TestConstants.Email
                };

                string groupResult = _groupService.LeaveGroup(leaveGroupRequest);

                Assert.AreEqual(AppConstants.Success, groupResult);
            } else {
                Assert.Fail();
            }
        }
    }
}
