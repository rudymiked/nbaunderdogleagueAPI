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

        //[TestMethod]
        //public void AddUsers()
        //{
        //    if (_userService != null) {
        //        List<UserEntity> userEntities = new();
        //        List<UserEntity> users = _userService.AddUsers(userEntities);

        //        Assert.AreNotEqual(0, users.Count);
        //    } else {
        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void GetUsersTest()
        //{
        //    if (_userService != null) {
        //        List<UserEntity> users = _userService.GetUsers();

        //        Assert.AreNotEqual(0, users.Count);
        //    } else {
        //        Assert.Fail();
        //    }
        //}
    }
}
