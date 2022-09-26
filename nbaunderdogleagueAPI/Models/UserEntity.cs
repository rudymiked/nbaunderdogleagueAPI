using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI
{
    public class UserEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Email { get; set; }
        public string Team { get; set; }
        public Guid League { get; set; }
        public int DraftOrder { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}