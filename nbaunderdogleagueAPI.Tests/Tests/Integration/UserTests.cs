using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class UserTests
    {
        private IUserService? _userService;

        [TestInitialize]
        public void SetUp()
        {
            _userService = new UserService();
        }

        [TestMethod]
        public void GetUsersTest()
        {
            if (_userService != null) {
                List<User> users = _userService.GetUsers();

                Assert.AreNotEqual(0, users.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
