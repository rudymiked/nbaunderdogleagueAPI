using Azure;
using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models
{
    public class CreateGroupResult
    {
        public GroupEntity GroupEntity { get; set; }
        public string Status { get; set; }
    }
}
