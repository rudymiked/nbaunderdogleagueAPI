using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class NBAGameEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string HomeLogo { get; set; }
        public string HomeTeam { get; set; }
        public int? HomeScore { get; set; }
        public string VisitorsLogo { get; set; }
        public string VisitorsTeam { get; set; }
        public int? VisitorsScore { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
