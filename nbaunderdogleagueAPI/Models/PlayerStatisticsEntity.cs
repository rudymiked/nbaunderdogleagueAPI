using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class PlayerStatisticsEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; } // "nickname"
        public string Position { get; set; } // "pos"
        public int TotalPoints { get; set; }
        public double AveragePoints { get; set; }
        public int MaxPoints { get; set; }
        public int MinPoints { get; set; }
        public int TotalMinutes { get; set; }
        public double AverageMinutes { get; set; }
        public int MaxMinutes { get; set; }
        public int MinMinutes { get; set; }
        public int TotalFieldGoalsMade { get; set; } // fgm
        public double AverageFieldGoalsMade { get; set; } // fgm
        public int MaxFieldGoalsMade { get; set; } // fgm
        public int MinFieldGoalsMade { get; set; } // fgm
        public int TotalFieldGoalAttempts { get; set; } // fga
        public double AverageFieldGoalAttempts { get; set; } // fga
        public int MaxFieldGoalAttempts { get; set; } // fga
        public int MinFieldGoalAttempts { get; set; } // fga
        public double FieldGoalPercentage { get; set; } // fgp
        public double MaxFieldGoalPercentage { get; set; } // fgp
        public double MinFieldGoalPercentage { get; set; } // fgp
        public int TotalFreeThrowsMade { get; set; } // ftm
        public double AverageFreeThrowsMade { get; set; } // ftm
        public int MaxFreeThrowsMade { get; set; } // ftm
        public int MinFreeThrowsMade { get; set; } // ftm
        public int TotalFreeThrowsAttempted { get; set; } //fta
        public double AverageFreeThrowsAttempted { get; set; } // fta
        public int MaxFreeThrowsAttempted { get; set; } // fta
        public int MinFreeThrowsAttempted { get; set; } // fta
        public double FreeThrowPercentage { get; set; } // fta
        public int TotalThreePointersMade { get; set; } // tpm
        public double AverageThreePointersMade { get; set; } // tpm
        public int MaxThreePointersMade { get; set; } // tpm
        public int MinThreePointersMade { get; set; } // tpm
        public int TotalThreePointersAttempted { get; set; } // tpa
        public double AverageThreePointersAttempted { get; set; } // tpa
        public int MaxThreePointersAttempted { get; set; } // tpa
        public int MinThreePointersAttempted { get; set; } // tpa
        public double ThreePointerPercentage { get; set; } //tpp
        public int TotalOffensiveRebounds { get; set; }
        public double AverageOffensiveRebounds { get; set; }
        public int MaxOffensiveRebounds { get; set; }
        public int MinOffensiveRebounds { get; set; }
        public int TotalDefensiveRebounds { get; set; }
        public double AverageDefensiveRebounds { get; set; }
        public int MaxDefensiveRebounds { get; set; }
        public int MinDefensiveRebounds { get; set; }
        public double AverageRebounds { get; set; }
        public int TotalRebounds { get; set; }
        public int TotalAssists { get; set; }
        public double AverageAssists { get; set; }
        public int MaxAssists { get; set; }
        public int MinAssists { get; set; }
        public int TotalFouls { get; set; }
        public double AverageFouls { get; set; }
        public int MaxFouls { get; set; }
        public int MinFouls { get; set; }
        public int TotalSteals { get; set; }
        public double AverageSteals { get; set; }
        public int MaxSteals { get; set; }
        public int MinSteals { get; set; }
        public int TotalTurnovers { get; set; }
        public double AverageTurnovers { get; set; }
        public int MaxTurnovers { get; set; }
        public int MinTurnovers { get; set; }
        public int TotalBlocks { get; set; }
        public double AverageBlocks { get; set; }
        public int MaxBlocks { get; set; }
        public int MinBlocks { get; set; }
        public double SeasonPlusMinus { get; set; }
        public double AveragePlusMinus { get; set; }
        public double MaxPlusMinus { get; set; }
        public double MinPlusMinus { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
