using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
                User user = new();
                User newUser = _userService.UpsertUser(user);

                Assert.AreEqual(newUser.Email, user.Email);
            } else {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetUsersTest()
        {
            if (_userService != null) {
                // need to query for a real league ID
                List<UserEntity> users = _userService.GetUsers(TestConstants.GroupId.ToString());

                Assert.AreNotEqual(0, users.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
