using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class TeamEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int? ID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int ProjectedWin { get; set; }
        public int ProjectedLoss { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public class ImportTeam
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int ProjectedWin { get; set; }
        public int ProjectedLoss { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string ETag { get; set; }
    }
}
