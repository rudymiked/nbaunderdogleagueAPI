using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI
{
    public class UserEntity : ITableEntity
    {
        public string PartitionKey { get; set; } // groupId
        public string RowKey { get; set; } // email
        public string Email { get; set; }
        public string Team { get; set; }
        public Guid Group { get; set; }
        public string Username { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}