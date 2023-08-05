using Azure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Tests.Integration
{
    [TestClass]
    public class PlayerTests
    {
        private IPlayerService _playerService;
        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => {
                    builder.ConfigureAppConfiguration((hostingContext, config) => {

                    });
                });

            _playerService = application.Services.GetService<IPlayerService>();
        }
    }
}
