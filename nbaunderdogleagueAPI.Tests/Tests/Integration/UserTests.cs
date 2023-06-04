using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class UserTests
    {
        private IUserService _userService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _userService = application.Services.GetService<IUserService>();
        }

        [TestMethod]
        public void UpsertUsers()
        {
            if (_userService != null) {
                User user = new() {
                    Email = TestConstants.Email,
                    Team = "Jazz",
                    Group = TestConstants.PostGroupId_TEST.ToString(),
                    Username = null,
                };
                User newUser = _userService.UpsertUser(user);

                Assert.AreEqual(newUser.Email, user.Email);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateUsername()
        {
            if (_userService != null) {
                User user = new() {
                    Email = TestConstants.Email,
                    Team = "Jazz",
                    Group = TestConstants.PostGroupId_TEST.ToString(),
                    Username = "New Username" + DateTime.Now.ToString(),
                };
                User newUser = _userService.UpsertUser(user);

                Assert.AreEqual(newUser.Email, user.Email);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetUsers()
        {
            if (_userService != null) {
                // need to query for a real league ID
                List<UserEntity> users = _userService.GetUsers(AppConstants.Group_2022.ToString());

                Assert.AreNotEqual(0, users.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
