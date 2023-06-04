using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class GroupInvitationEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public Guid InviteId { get; set; }
        public Guid GroupId { get; set; }
        public string Email { get; set; }
        public bool Approved { get; set; }
        public bool Declined { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
