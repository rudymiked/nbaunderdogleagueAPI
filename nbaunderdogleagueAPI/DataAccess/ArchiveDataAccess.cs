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
        List<SeasonArchiveEntity> UpdateArchives();
        SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity userArchive);
        List<ArchiveSummary> GetArchiveSummary(string email);
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
                    TeamStats teamStats = teamEntities.FirstOrDefault(team => team.TeamName == standings.TeamName);

                    seasonArchiveEntities.Add(new SeasonArchiveEntity() {
                        PartitionKey = groupId,
                        RowKey = standings.Email,
                        ETag = ETag.All,
                        Timestamp = DateTime.UtcNow,
                        GroupId = groupId,
                        Year = AppConstants.CurrentNBASeasonYear,
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
                        Score = Utils.CalculateScore(standings.ProjectedWin, standings.ProjectedLoss, standings.Win, standings.Loss, standings.PlayoffWins),
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

                TeamStats teamStats = teamEntities.FirstOrDefault(team => team.TeamName == seasonArchiveEntity.TeamName);

                seasonArchiveEntity.TeamID = teamStats != null ? teamStats.TeamID : 0;
                seasonArchiveEntity.Score = Utils.CalculateScore(seasonArchiveEntity.ProjectedWin, seasonArchiveEntity.ProjectedLoss, seasonArchiveEntity.Wins, seasonArchiveEntity.Losses, seasonArchiveEntity.PlayoffWins);

                var response = _tableStorageHelper.UpsertEntities(new List<SeasonArchiveEntity>() { seasonArchiveEntity }, AppConstants.ArchiveTable).Result;

                return (response != null && !response.GetRawResponse().IsError) ? seasonArchiveEntity : new SeasonArchiveEntity();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new SeasonArchiveEntity();
        }

        public List<ArchiveSummary> GetArchiveSummary(string email)
        {
            try {
                // 1. Query Archive data
                List<SeasonArchiveEntity> seasonArchiveEntities = GetAllUserArchives(email);

                // 2. Query Group data
                List<GroupEntity> groupEntities = _groupService.GetAllGroups();

                List<ArchiveSummary> archiveSummaries = new();

                foreach (SeasonArchiveEntity archive in seasonArchiveEntities) {
                    archiveSummaries.Add(new ArchiveSummary() {
                        Year = groupEntities.FirstOrDefault(group => group.Id.ToString() == archive.GroupId)?.Year,
                        Email = archive.Email,
                        Governor = archive.Governor,
                        GroupId = archive.GroupId,
                        GroupName = groupEntities.FirstOrDefault(group => group.Id.ToString() == archive.GroupId)?.Name,
                        Score = archive.Score,
                        TeamCity = archive.TeamCity,
                        TeamName = archive.TeamName,
                        Standing = archive.Standing
                    });
                }

                return archiveSummaries;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<ArchiveSummary>();
        }

        List<SeasonArchiveEntity> GetAllUserArchives(string email)
        {
            string filter = TableClient.CreateQueryFilter<SeasonArchiveEntity>((group) => group.Email == email);

            var response = _tableStorageHelper.QueryEntities<SeasonArchiveEntity>(AppConstants.ArchiveTable, filter).Result;

            return response.Any() ? response.ToList() : new List<SeasonArchiveEntity>();
        }

        public List<SeasonArchiveEntity> UpdateArchives()
        {
            try {
                int updatedCount = 0;

                List<SeasonArchiveEntity> resultList = new();
                List<GroupEntity> groups = _groupService.GetAllGroups();

                foreach (GroupEntity group in groups) {
                    List<SeasonArchiveEntity> seasonArchiveEntities = GetSeasonArchive(group.Id.ToString());

                    if (seasonArchiveEntities.Count == 0) {
                        continue;
                    }

                    for (int i = 0; i < seasonArchiveEntities.Count; i++) {
                        int wins = seasonArchiveEntities[i].Wins;
                        int losses = seasonArchiveEntities[i].Losses;
                        int projectedWins = seasonArchiveEntities[i].ProjectedWin;
                        int projectedLosses = seasonArchiveEntities[i].ProjectedLoss;
                        int? playoffWins = seasonArchiveEntities[i].PlayoffWins;

                        seasonArchiveEntities[i].Year = group.Year;
                        seasonArchiveEntities[i].GroupId = group.Id.ToString();
                        seasonArchiveEntities[i].Score = Utils.CalculateScore(projectedWins, projectedLosses, wins, losses, playoffWins);
                    }

                    // order by score to get standings
                    seasonArchiveEntities = seasonArchiveEntities.OrderByDescending(season => season.Score).ToList();

                    for (int i = 0; i < seasonArchiveEntities.Count; i++) {
                        // set same standing is user's have the same score
                        if (i > 0 && seasonArchiveEntities[i - 1].Score == seasonArchiveEntities[i].Score) {
                            seasonArchiveEntities[i].Standing = seasonArchiveEntities[i - 1].Standing;
                        } else {
                            seasonArchiveEntities[i].Standing = i + 1;
                        }
                    }

                    resultList.AddRange(seasonArchiveEntities);

                    var response = _tableStorageHelper.UpsertEntities(seasonArchiveEntities, AppConstants.ArchiveTable).Result;

                    updatedCount += (response != null && !response.GetRawResponse().IsError) ? seasonArchiveEntities.Count : 0;
                }

                return updatedCount == resultList.Count ? resultList : new List<SeasonArchiveEntity>();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }
            return new List<SeasonArchiveEntity>();
        }
    }
}
