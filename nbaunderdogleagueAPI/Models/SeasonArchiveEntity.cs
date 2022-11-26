using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class SeasonArchiveEntity : ITableEntity
    {
        public string PartitionKey { get; set; } // groupId
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Governor { get; set; }
        public string Email { get; set; }
        public int TeamID { get; set; }
        public string TeamCity { get; set; }
        public string TeamName { get; set; }
        public int Standing { get; set; }
        public int ProjectedWin { get; set; }
        public int ProjectedLoss { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int? ClinchedPlayoffBirth { get; set; }
        public int PlayoffWins { get; set; }
    }
}
