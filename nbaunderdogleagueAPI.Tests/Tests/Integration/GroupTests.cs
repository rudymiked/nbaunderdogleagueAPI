using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        public void GetGroupStandingsTest()
        {
            if (_groupService != null) {
                List<GroupStandings> standings = _groupService.GetGroupStandings(TestConstants.GroupId.ToString());

                Assert.AreNotEqual(0, standings.Count);
            } else {
                Assert.Fail();
            }
        }        

        [TestMethod]
        public void CreateGroup()
        {
            if (_groupService != null) {
                GroupEntity newGroup = _groupService.CreateGroup("Black Lung", TestConstants.Email);

                Assert.AreNotEqual(string.Empty, newGroup.Id);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetGroup()
        {
            if (_groupService != null) {
                GroupEntity group = _groupService.GetGroup(TestConstants.GroupId.ToString());

                Assert.AreNotEqual(string.Empty, group.Name);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAllGroupsByYear()
        {
            if (_groupService != null) {
                List<GroupEntity> groups = _groupService.GetAllGroupsByYear(DateTime.Now.Year, true, TestConstants.Email);

                Assert.AreEqual(true, groups.Any());
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAllGroupsUserIsInByYear()
        {
            if (_groupService != null) {
                List<GroupEntity> groups = _groupService.GetAllGroupsUserIsInByYear(TestConstants.Email, DateTime.Now.Year);

                Assert.AreEqual(true, groups.Any());
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void JoinGroup()
        {
            if (_groupService != null) {
                // need to query for an existing league first
                // also need a user email
                string groupResult = _groupService.JoinGroup(TestConstants.GroupId.ToString(), TestConstants.Email);

                Assert.AreNotEqual(AppConstants.Success, groupResult);
            } else {
                Assert.Fail();
            }
        }
    }
}
