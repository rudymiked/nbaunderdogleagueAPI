using Azure;
using Azure.Data.Tables;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IArchiveDataAccess
    {
        List<SeasonArchiveEntity> ArchiveCurrentSeason(string groupId);
        List<SeasonArchiveEntity> GetSeasonArchive(string groupId);
        SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity userArchive);
    }
    public class ArchiveDataAccess : IArchiveDataAccess
    {
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly IGroupService _groupService;
        private readonly ITeamService _teamService;
        private readonly ILogger _logger;

        public ArchiveDataAccess(ITableStorageHelper tableStorageHelper, IGroupService groupService, ITeamService teamService, ILogger<ArchiveDataAccess> logger)
        {
            _tableStorageHelper = tableStorageHelper;
            _groupService = groupService;
            _teamService = teamService;
            _logger = logger;
        }
        public List<SeasonArchiveEntity> ArchiveCurrentSeason(string groupId)
        {
            int version = 2; // 2 is manual data

            try {
                // 1. Query current season data

                List<GroupStandings> groupStandings = _groupService.GetGroupStandings(groupId, version);

                // Query team data (to collect team ID)

                List<TeamStats> teamEntities = _teamService.TeamStatsList(version);

                // 2. Upsert Archive data

                List<SeasonArchiveEntity> seasonArchiveEntities = new();

                foreach (GroupStandings standings in groupStandings) {
                    TeamStats teamStats = teamEntities.Where(team => team.TeamName == standings.TeamName).FirstOrDefault();

                    seasonArchiveEntities.Add(new SeasonArchiveEntity() {
                        PartitionKey = groupId,
                        RowKey = standings.Email,
                        ETag = ETag.All,
                        Timestamp = DateTime.UtcNow,
                        TeamID = teamStats != null ? teamStats.TeamID : 0,
                        TeamCity = standings.TeamCity,
                        TeamName = standings.TeamName,
                        Governor = standings.Governor,
                        Email = standings.Email,
                        Standing = 0,
                        ProjectedWin = standings.ProjectedWin,
                        ProjectedLoss = standings.ProjectedLoss,
                        Wins = standings.Win,
                        Losses = standings.Loss,
                        PlayoffWins = standings.PlayoffWins,
                        ClinchedPlayoffBirth = standings.Playoffs == "Yes" ? 1 : 0
                    });
                }

                var response = _tableStorageHelper.UpsertEntities(seasonArchiveEntities, AppConstants.ArchiveTable).Result;

                return (response != null && !response.GetRawResponse().IsError) ? seasonArchiveEntities : new List<SeasonArchiveEntity>();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<SeasonArchiveEntity>();
        }

        public List<SeasonArchiveEntity> GetSeasonArchive(string groupId)
        {
            string filter = TableClient.CreateQueryFilter<SeasonArchiveEntity>((group) => group.PartitionKey == groupId);

            var response = _tableStorageHelper.QueryEntities<SeasonArchiveEntity>(AppConstants.ArchiveTable, filter).Result;

            return response.Any() ? response.ToList() : new List<SeasonArchiveEntity>();
        }

        public SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity seasonArchiveEntity)
        {
            try {
                int version = 2; // manual data
                                 // Query team data (to collect team ID)

                List<TeamStats> teamEntities = _teamService.TeamStatsList(version);

                TeamStats teamStats = teamEntities.Where(team => team.TeamName == seasonArchiveEntity.TeamName).FirstOrDefault();

                seasonArchiveEntity.TeamID = teamStats != null ? teamStats.TeamID : 0;

                var response = _tableStorageHelper.UpsertEntities(new List<SeasonArchiveEntity>() { seasonArchiveEntity }, AppConstants.ArchiveTable).Result;

                return (response != null && !response.GetRawResponse().IsError) ? seasonArchiveEntity : new SeasonArchiveEntity();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new SeasonArchiveEntity();
        }
    }
}
