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
        public void ArchiveCurrentSeason()
        {
            List<SeasonArchiveEntity> seasonArchiveEntities = _archiveService.ArchiveCurrentSeason(AppConstants.Group_2022.ToString());

            Assert.AreNotEqual(0, seasonArchiveEntities.Count);
        }

        [TestMethod]
        public void ArchiveUser()
        {
            SeasonArchiveEntity userArchive = new() {
                PartitionKey = AppConstants.Group_2019.ToString(),
                RowKey = "will@gmail.com",
                ETag = ETag.All,
                Timestamp = DateTime.UtcNow,
                TeamID = 0,
                TeamCity = "Portland",
                TeamName = "Trail Blazers",
                Governor = "Will",
                Email = "will@gmail.com",
                Standing = 12,
                ProjectedWin = 40,
                ProjectedLoss = 42,
                Wins = 26,
                Losses = 33,
                PlayoffWins = 1,
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
        public void GetArchiveSummary()
        {
            List<ArchiveSummary> archiveSummaries = _archiveService.GetArchiveSummary(TestConstants.Email);

            Assert.AreNotEqual(0, archiveSummaries.Count);
        }

        [TestMethod]
        public void UpdateArchives()
        {
            List<SeasonArchiveEntity> updatedArchives = _archiveService.UpdateArchives();

            Assert.AreNotEqual(0, updatedArchives.Count);
        }
    }
}
