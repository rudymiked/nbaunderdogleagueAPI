using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class TeamTests
    {
        private ITeamService? _teamService;

        [TestInitialize]
        public void SetUp()
        {
            _teamService = new TeamService();
        }

        [TestMethod]
        public void GetTeamsTest()
        {
            if (_teamService != null) {
                List<Team> users = _teamService.GetTeams();

                Assert.AreNotEqual(0, users.Count);
            } else {
                Assert.Fail();
            }
        }
    }
}
