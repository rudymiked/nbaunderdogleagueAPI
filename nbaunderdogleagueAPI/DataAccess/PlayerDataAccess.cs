using Azure;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Models.PlayerStatistics;
using nbaunderdogleagueAPI.Models.RapidAPI_NBA;
using Newtonsoft.Json;
using static nbaunderdogleagueAPI.Models.PlayerStatistics.PlayerStatistics;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IPlayerDataAccess
    {
        public PlayerResponse GetAllPlayerStatsFromRapidAPI(int season);
        public PlayerResponse GetPlayerStatsPerTeamFromRapidAPI(int teamId, int season);
        public List<PlayerStatisticsEntity> UpdatePlayerStatsFromRapidAPI(int teamId = 0, int season = 0);
        public List<PlayerStatisticsEntity> GetPlayerStatistics();
    }
    public class PlayerDataAccess : IPlayerDataAccess
    {
        private readonly ILogger _logger;
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly IRapidAPIHelper _rapidAPIHelper;
        private readonly AppConfig _appConfig;
        public PlayerDataAccess(IOptions<AppConfig> appConfig, ILogger<NBADataAccess> logger, ITableStorageHelper tableStorageHelper, IRapidAPIHelper rapidAPIHelper)
        {
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
            _rapidAPIHelper = rapidAPIHelper;
            _appConfig = appConfig.Value;
        }

        private List<PlayerStatisticsEntity> GetCompiledPlayerStatistics(int teamId = 0, int season = 0)
        {
            try {
                PlayerResponse playerResponse = teamId == 0 
                                                ? GetAllPlayerStatsFromRapidAPI(season)
                                                : GetPlayerStatsPerTeamFromRapidAPI(teamId, season);


                Dictionary<int, PlayerStatisticsEntity> playerStatsDictionary = new();
                Dictionary<int, int> numberOfGamePerPlayer = new();

                foreach (PlayerStatistics.Response player in playerResponse.Players) {
                    // need to check if we've seen this player before, so we can caluate min,max,avg,etc.
                    PlayerStatisticsEntity previousPlayerStatistics = playerStatsDictionary.TryGetValue(player.Player.Id, out PlayerStatisticsEntity playerData) 
                                                                        ? playerData
                                                                        : null;

                    if (numberOfGamePerPlayer.TryGetValue(player.Player.Id, out int numberOfGames)) {
                        numberOfGamePerPlayer[player.Player.Id] = numberOfGames + 1;
                    } else {
                        numberOfGamePerPlayer.Add(player.Player.Id, 1);
                    }

                    if (player.Min == "--") {
                        // player did not play
                        continue;
                    }

                    int playerMinutes = int.TryParse(player.Min, out int m) ? m : 0;
                    double playerPlusMinus = double.TryParse(player.PlusMinus, out double pm) ? pm : 0;
                    double playerFGP = double.TryParse(player.Fgp, out double fpg) ? fpg : 0;
                    double playerFTP = double.TryParse(player.Ftp, out double ftp) ? ftp : 0;
                    double playerTPP = double.TryParse(player.Tpp, out double tpp) ? tpp : 0;
                    
                    PlayerStatisticsEntity nextPlayer = new() {
                        PartitionKey = season.ToString(),
                        RowKey = player.Player.Id.ToString(),
                        PlayerId = player.Player.Id,
                        PlayerName = player.Player.Firstname + " " + player.Player.Lastname,
                        TeamName = player.Team.Nickname, // "nickname" aka "Hawks"
                        Position = player.Pos, // "pos"

                        // Totals
                        TotalPoints = player.Points + (previousPlayerStatistics?.TotalPoints ?? 0),
                        TotalMinutes = playerMinutes + (previousPlayerStatistics?.TotalMinutes ?? 0),
                        TotalFieldGoalsMade = player.Fgm + (previousPlayerStatistics?.TotalFieldGoalsMade ?? 0), // fgm
                        TotalFieldGoalAttempts = player.Fga + (previousPlayerStatistics?.TotalFieldGoalAttempts ?? 0), // fga
                        TotalFreeThrowsMade = player.Ftm + (previousPlayerStatistics?.TotalFreeThrowsMade ?? 0), // ftm
                        TotalFreeThrowsAttempted = player.Fta + (previousPlayerStatistics?.TotalFreeThrowsAttempted ?? 0), //fta
                        TotalThreePointersMade = player.Tpm + (previousPlayerStatistics?.TotalThreePointersMade ?? 0), // tpm
                        TotalThreePointersAttempted = player.Tpa + (previousPlayerStatistics?.TotalThreePointersAttempted ?? 0), // tpa
                        TotalOffensiveRebounds = player.OffReb + (previousPlayerStatistics?.TotalOffensiveRebounds ?? 0),
                        TotalDefensiveRebounds = player.DefReb + (previousPlayerStatistics?.TotalDefensiveRebounds ?? 0),
                        TotalRebounds = player.TotReb + (previousPlayerStatistics?.TotalRebounds ?? 0),
                        TotalAssists = player.Assists + (previousPlayerStatistics?.TotalAssists ?? 0),
                        TotalFouls = player.PFouls + (previousPlayerStatistics?.TotalFouls ?? 0),
                        TotalTurnovers = player.Turnovers + (previousPlayerStatistics?.TotalTurnovers ?? 0),
                        TotalSteals = player.Steals + (previousPlayerStatistics?.TotalSteals ?? 0),
                        TotalBlocks = player.Blocks + (previousPlayerStatistics?.TotalBlocks ?? 0),
                        SeasonPlusMinus = playerPlusMinus + (previousPlayerStatistics?.SeasonPlusMinus ?? 0),

                        // Averages
                        AveragePoints = PlayerUtils.Average(player.Points, previousPlayerStatistics?.AveragePoints, numberOfGames),
                        AverageMinutes = PlayerUtils.Average(playerMinutes, previousPlayerStatistics?.AverageMinutes, numberOfGames),
                        AverageFieldGoalsMade = PlayerUtils.Average(player.Fgm, previousPlayerStatistics?.AverageFieldGoalsMade, numberOfGames), // fgm
                        AverageFieldGoalAttempts = PlayerUtils.Average(player.Fga, previousPlayerStatistics?.AverageFieldGoalAttempts, numberOfGames), // fga
                        AverageFreeThrowsMade = PlayerUtils.Average(player.Ftm, previousPlayerStatistics?.AverageFreeThrowsMade, numberOfGames), // ftm
                        AverageFreeThrowsAttempted = PlayerUtils.Average(player.Fta, previousPlayerStatistics?.AverageFreeThrowsAttempted, numberOfGames), // fta
                        AverageThreePointersMade = PlayerUtils.Average(player.Tpm, previousPlayerStatistics?.AverageThreePointersMade, numberOfGames), // tpm
                        AverageThreePointersAttempted = PlayerUtils.Average(player.Tpa, previousPlayerStatistics?.AverageThreePointersAttempted, numberOfGames), // tpa
                        AverageOffensiveRebounds = PlayerUtils.Average(player.OffReb, previousPlayerStatistics?.AverageOffensiveRebounds, numberOfGames),
                        AverageDefensiveRebounds = PlayerUtils.Average(player.DefReb, previousPlayerStatistics?.AverageDefensiveRebounds, numberOfGames),
                        AverageRebounds = PlayerUtils.Average(player.TotReb, previousPlayerStatistics?.AverageRebounds, numberOfGames),
                        AverageAssists = PlayerUtils.Average(player.Assists, previousPlayerStatistics?.AverageAssists, numberOfGames),
                        AverageFouls = PlayerUtils.Average(player.PFouls, previousPlayerStatistics?.AverageFouls, numberOfGames),
                        AverageBlocks = PlayerUtils.Average(player.Blocks, previousPlayerStatistics?.AverageBlocks, numberOfGames),
                        AverageSteals = PlayerUtils.Average(player.Steals, previousPlayerStatistics?.AverageSteals, numberOfGames),
                        AverageTurnovers = PlayerUtils.Average(player.Turnovers, previousPlayerStatistics?.AverageTurnovers, numberOfGames),
                        FieldGoalPercentage = PlayerUtils.Average(playerFGP, previousPlayerStatistics?.FieldGoalPercentage, numberOfGames), // fgp
                        FreeThrowPercentage = PlayerUtils.Average(playerFTP, previousPlayerStatistics?.FreeThrowPercentage, numberOfGames), // ftp
                        ThreePointerPercentage = PlayerUtils.Average(playerTPP, previousPlayerStatistics?.ThreePointerPercentage, numberOfGames), //tpp
                        AveragePlusMinus = PlayerUtils.Average(playerPlusMinus, previousPlayerStatistics?.AveragePlusMinus, numberOfGames),

                        // Maximums
                        MaxPoints = PlayerUtils.Max(player.Points, previousPlayerStatistics?.MaxPoints),
                        MaxMinutes = PlayerUtils.Max(player.Points, previousPlayerStatistics?.MaxMinutes),
                        MaxFieldGoalsMade = PlayerUtils.Max(player.Fgm, previousPlayerStatistics?.MaxFieldGoalsMade), // fgm
                        MaxThreePointersMade = PlayerUtils.Max(player.Tpm, previousPlayerStatistics?.MaxThreePointersMade), // tpm
                        MaxFreeThrowsAttempted = PlayerUtils.Max(player.Fta, previousPlayerStatistics?.MaxFreeThrowsAttempted), // fta
                        MaxFieldGoalPercentage = PlayerUtils.Max(playerFGP, previousPlayerStatistics?.MaxFieldGoalPercentage), // fgp
                        MaxFieldGoalAttempts = PlayerUtils.Max(player.Fga, previousPlayerStatistics?.MaxFieldGoalAttempts), // fga
                        MaxFreeThrowsMade = PlayerUtils.Max(player.Ftm, previousPlayerStatistics?.MaxFreeThrowsMade), // ftm
                        MaxThreePointersAttempted = PlayerUtils.Max(player.Tpa, previousPlayerStatistics?.MaxThreePointersAttempted), // tpa
                        MaxOffensiveRebounds = PlayerUtils.Max(player.OffReb, previousPlayerStatistics?.MaxOffensiveRebounds),
                        MaxDefensiveRebounds = PlayerUtils.Max(player.DefReb, previousPlayerStatistics?.MaxDefensiveRebounds),
                        MaxAssists = PlayerUtils.Max(player.Assists, previousPlayerStatistics?.MaxAssists),
                        MaxFouls = PlayerUtils.Max(player.PFouls, previousPlayerStatistics?.MaxFouls),
                        MaxSteals = PlayerUtils.Max(player.Steals, previousPlayerStatistics?.MaxSteals),
                        MaxTurnovers = PlayerUtils.Max(player.Turnovers, previousPlayerStatistics?.MaxTurnovers),
                        MaxBlocks = PlayerUtils.Max(player.Blocks, previousPlayerStatistics?.MaxBlocks),
                        MaxPlusMinus = PlayerUtils.Max(playerPlusMinus, previousPlayerStatistics?.MaxPlusMinus),

                        // Minimums
                        MinPoints = PlayerUtils.Min(player.Points, previousPlayerStatistics?.MinPoints),
                        MinMinutes = PlayerUtils.Min(playerMinutes, previousPlayerStatistics?.MinMinutes),
                        MinFieldGoalsMade = PlayerUtils.Min(player.Fgm, previousPlayerStatistics?.MinFieldGoalsMade), // fgm
                        MinFieldGoalAttempts = PlayerUtils.Min(player.Fga, previousPlayerStatistics?.MinFieldGoalAttempts), // fga
                        MinFieldGoalPercentage = PlayerUtils.Min(playerFGP, previousPlayerStatistics?.MinFieldGoalPercentage), // fgp
                        MinFreeThrowsMade = PlayerUtils.Min(player.Ftm, previousPlayerStatistics?.MinFreeThrowsMade), // ftm
                        MinFreeThrowsAttempted = PlayerUtils.Min(player.Fta, previousPlayerStatistics?.MinFreeThrowsAttempted), // fta
                        MinThreePointersMade = PlayerUtils.Min(player.Tpm, previousPlayerStatistics?.MinThreePointersMade), // tpm
                        MinThreePointersAttempted = PlayerUtils.Min(player.Tpa, previousPlayerStatistics?.MinThreePointersAttempted), // tpa
                        MinOffensiveRebounds = PlayerUtils.Min(player.OffReb, previousPlayerStatistics?.MinOffensiveRebounds),
                        MinDefensiveRebounds = PlayerUtils.Min(player.DefReb, previousPlayerStatistics?.MinDefensiveRebounds),
                        MinAssists = PlayerUtils.Min(player.Assists, previousPlayerStatistics?.MinAssists),
                        MinFouls = PlayerUtils.Min(player.PFouls, previousPlayerStatistics?.MinFouls),
                        MinSteals = PlayerUtils.Min(player.Steals, previousPlayerStatistics?.MinSteals),
                        MinTurnovers = PlayerUtils.Min(player.Turnovers, previousPlayerStatistics?.MinTurnovers),
                        MinBlocks = PlayerUtils.Min(player.Blocks, previousPlayerStatistics?.MinBlocks),
                        MinPlusMinus = PlayerUtils.Min(playerPlusMinus, previousPlayerStatistics?.MinPlusMinus),

                        Timestamp = DateTimeOffset.UtcNow,
                        ETag = ETag.All
                    };

                    if (playerStatsDictionary.ContainsKey(player.Player.Id)) {
                        playerStatsDictionary[player.Player.Id] = nextPlayer;
                    } else {
                        playerStatsDictionary.Add(player.Player.Id, nextPlayer);
                    }
                }

                return playerStatsDictionary.Select(x => x.Value).ToList();
            } 
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public PlayerResponse GetAllPlayerStatsFromRapidAPI(int season = 0)
        {
            try {
                PlayerResponse playerResponse = new() {
                    Players = new()
                };

                for (int i = 1; i <= AppConstants.NumberOfNBATeams; ++i) {
                    PlayerResponse newTeamResponse = GetPlayerStatsPerTeamFromRapidAPI(i, season);

                    playerResponse.Players.AddRange(newTeamResponse.Players);

                    if (newTeamResponse.RequestsRemaining == 0) {
                        _rapidAPIHelper.SetRapidAPITimeout(DateTimeOffset.UtcNow.AddDays(1));
                        break;
                    }

                    Thread.Sleep(10000); // can only query 10 times per minute. Setting sleep to every 10 seconds to be safe.
                }

                return playerResponse; // all players all teams. 
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public PlayerResponse GetPlayerStatsPerTeamFromRapidAPI(int teamId = 0, int season = 0)
        {
            try {
                teamId = teamId == 0 ? 1 : teamId; // if teamID not supplied, default to team #1
                int seasonQuery = season == 0 ? AppConstants.CurrentNBASeasonYear : season;

                string apiURL = "https://api-nba-v1.p.rapidapi.com/players/statistics";
                string parameterString = "?team=" + teamId + "&season=" + seasonQuery;

                Root output;

                RapidAPI_NBA.RapidAPIContent content = _rapidAPIHelper.QueryRapidAPI(apiURL, parameterString).Result;

                if (string.IsNullOrEmpty(content.Content)) {
                    return new PlayerResponse();
                }

                output = JsonConvert.DeserializeObject<Root>(content.Content, new JsonSerializerSettings {  NullValueHandling = NullValueHandling.Ignore});

                List<PlayerStatistics.Response> players = output.Response;

                return new PlayerResponse() {
                    Players = players,
                    RequestsRemaining = content.RequestsRemaining
                };
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public List<PlayerStatisticsEntity> UpdatePlayerStatsFromRapidAPI(int teamId = 0, int season = 0)
        {
            // Rapid API request limit has been met
            // do not update
            if (!_rapidAPIHelper.IsRapidAPIAvailable()) {
                return new List<PlayerStatisticsEntity>();
            }

            try {
                List<PlayerStatisticsEntity> players = GetCompiledPlayerStatistics(teamId, season);
                // replace current players in table storage
                if (players.Count > 0) {
                    List<PlayerStatisticsEntity> currentPlayerStatistics = _tableStorageHelper.QueryEntities<PlayerStatisticsEntity>(AppConstants.PlayerStatisticsTable)
                                                            .Result
                                                            .ToList();

                    _tableStorageHelper.DeleteAllEntities(currentPlayerStatistics.ToList(), AppConstants.PlayerStatisticsTable);

                    var updateGamesResponse = _tableStorageHelper.UpsertEntities(players, AppConstants.PlayerStatisticsTable).Result;

                    return (updateGamesResponse == AppConstants.Success) ? players : new List<PlayerStatisticsEntity>();
                } else {
                    _logger.LogInformation("Could not load player stats: " + DateTime.Now.ToString());
                }
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public List<PlayerStatisticsEntity> GetPlayerStatistics()
        {
            try {
                return _tableStorageHelper.QueryEntities<PlayerStatisticsEntity>(AppConstants.PlayerStatisticsTable).Result.ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}
