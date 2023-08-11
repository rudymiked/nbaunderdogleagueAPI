using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Tests.Integration
{
    [TestClass]
    public class RapidAPIHelperTests
    {
        private IRapidAPIHelper _rapidAPIHelper;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _rapidAPIHelper = application.Services.GetService<IRapidAPIHelper>();
        }

        //[TestMethod]
        //public void SetRapidAPITimeout()
        //{
        //    if (_nbaService != null) {
        //        DateTimeOffset now = DateTimeOffset.UtcNow;
        //        bool setTimeoutResult = _nbaService.SetRapidAPITimeout(now.AddDays(1));

        //        Assert.AreEqual(true, setTimeoutResult);
        //    } else {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public void IsRapidAPIAvailable()
        {
            if (_rapidAPIHelper != null) {
                try {
                    bool rapidAPIAvailable = _rapidAPIHelper.IsRapidAPIAvailable();

                    Assert.IsTrue(true);
                } catch (Exception) {
                    Assert.Fail();
                }
            } else {
                Assert.Fail();
            }
        }
    }
}

