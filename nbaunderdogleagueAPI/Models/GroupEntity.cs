using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class GroupEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public Guid Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public DateTimeOffset DraftDate { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
