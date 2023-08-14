using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Integration
{
    [TestClass]
    public class EmailTests
    {
        private IEmailService _emailService;

        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration((hostingContext, config) => {

                });
            });

            _emailService = application.Services.GetService<IEmailService>();
        }

        [TestMethod]
        public async Task InviteUserToGroupTestAsync()
        {
            if (_emailService != null) {

                var emailResponse = await _emailService.InviteUserToGroup(TestConstants.Email, TestConstants.PostGroupId_TEST.ToString());

                Assert.IsTrue(emailResponse.IsSuccessStatusCode);
            } else {
                Assert.Fail();
            }
        }
    }
}
