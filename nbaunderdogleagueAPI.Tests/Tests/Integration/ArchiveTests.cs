using Azure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Tests.Tests.Integration
{
    [TestClass]
    public class ArchiveTests
    {
        private IArchiveService _archiveService;
        [TestInitialize]
        public void SetUp()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => {
                    builder.ConfigureAppConfiguration((hostingContext, config) => {

                    });
                });

            _archiveService = application.Services.GetService<IArchiveService>();
        }

        [TestMethod]
        public void ArchiveCurrentSeasonTest()
        {
            List<SeasonArchiveEntity> seasonArchiveEntities = _archiveService.ArchiveCurrentSeason(AppConstants.Group_2022.ToString());

            Assert.AreNotEqual(0, seasonArchiveEntities.Count);
        }

        [TestMethod]
        public void ArchiveUser()
        {
            SeasonArchiveEntity userArchive = new() {
                PartitionKey = AppConstants.Group_2018.ToString(),
                RowKey = "davidhigh@gmail.com",
                ETag = ETag.All,
                Timestamp = DateTime.UtcNow,
                TeamID = 0,
                TeamCity = "LA",
                TeamName = "Clippers",
                Governor = "D High",
                Email = "davidhigh@gmail.com",
                Standing = 5,
                ProjectedWin = 41,
                ProjectedLoss = 41,
                Wins = 38,
                Losses = 44,
                PlayoffWins = 2,
                ClinchedPlayoffBirth = 1
            };

            SeasonArchiveEntity seasonArchive = _archiveService.ArchiveUser(userArchive);

            Assert.AreNotEqual(null, seasonArchive);
        }

        [TestMethod]
        public void GetSeasonArchive()
        {
            List<SeasonArchiveEntity> seasonArchiveEntities = _archiveService.GetSeasonArchive(AppConstants.Group_2019.ToString());

            Assert.AreNotEqual(0, seasonArchiveEntities.Count);
        }
        [TestMethod]
        public void GetArchiveSummaryTest()
        {
            List<ArchiveSummary> archiveSummaries = _archiveService.GetArchiveSummary(TestConstants.Email);

            Assert.AreNotEqual(0, archiveSummaries.Count);
        }
    }
}
