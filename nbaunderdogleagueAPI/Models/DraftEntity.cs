using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class DraftEntity : ITableEntity
    {
        public string PartitionKey { get; set; } // league ID
        public string RowKey { get; set; } // draft ID
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Email { get; set; }
        public int DraftOrder { get; set; }
        public DateTimeOffset UserStartTime { get; set; }
        public DateTimeOffset UserEndTime { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
