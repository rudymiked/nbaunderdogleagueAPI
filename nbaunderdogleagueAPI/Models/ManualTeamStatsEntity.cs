using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class ManualTeamStatsEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string TeamCity { get; set; }
        public string Conference { get; set; }
        public int Standing { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double Ratio { get; set; }
        public int? Streak { get; set; }
        public int? ClinchedPlayoffBirth { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
