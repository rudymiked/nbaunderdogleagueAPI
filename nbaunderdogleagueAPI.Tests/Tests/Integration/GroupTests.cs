using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA.RapidAPI_NBA.Game;

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
                List<GroupStandings> standings = _groupService.GetGroupStandings("938d60e2-0144-436d-ae6a-5357df703aa4", 2);

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
        public void GetGroupAndUpdate()
        {
            if (_groupService != null) {
                GroupEntity group = _groupService.GetGroup(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(string.Empty, group.Name);

                group.Timestamp = DateTimeOffset.UtcNow;

                GroupEntity updateGroup = _groupService.UpsertGroup(group);

                Assert.IsNotNull(updateGroup);

                group = _groupService.GetGroup(AppConstants.Group_2022.ToString());

                TimeSpan timeDifference = (TimeSpan)(group.Timestamp - DateTimeOffset.UtcNow);

                Assert.IsTrue(Math.Abs(timeDifference.TotalMinutes) <= 5);
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

        [TestMethod] 
        public void GetJoinGroupRequests()
        {
            if (_groupService != null) {
                JoinGroupRequest joinGroupRequest = new() {
                    GroupId = TestConstants.PostGroupId_TEST.ToString(),
                    Email = TestConstants.Email
                };

                // request to join
                string groupResult = _groupService.JoinGroup(joinGroupRequest);

                Assert.AreEqual(AppConstants.Success, groupResult);

                // ensure request is present
                List<JoinGroupRequestEntity> joinGroupRequestEntity = _groupService.GetJoinGroupRequests(TestConstants.PostGroupId_TEST.ToString());

                Assert.IsTrue(joinGroupRequestEntity.Select(x => x.GroupId == TestConstants.PostGroupId_TEST.ToString() && x.Email == TestConstants.Email).Any());

                // approve request
                ApproveUserRequest approveUserRequest = new() {
                    GroupId = TestConstants.PostGroupId_TEST.ToString(),
                    Email = TestConstants.Email,
                    InviteId = null,
                    AdminEmail = TestConstants.NBAEmail
                };

                string approval = _groupService.ApproveNewGroupMember(approveUserRequest);

                Assert.AreEqual(AppConstants.Success, approval);

                // leave group
                LeaveGroupRequest leaveGroupRequest = new() {
                    GroupId = TestConstants.PostGroupId_TEST.ToString(),
                    Email = TestConstants.Email
                };

                string leaveGroupResult = _groupService.LeaveGroup(leaveGroupRequest);

                Assert.AreEqual(AppConstants.Success, leaveGroupResult);
            }
        }
    }
}
