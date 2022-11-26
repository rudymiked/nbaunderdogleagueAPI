namespace nbaunderdogleagueAPI.Models
{
    public class DraftResults
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Email { get; set; }
        public int DraftOrder { get; set; }
        public DateTimeOffset UserStartTime { get; set; }
        public DateTimeOffset UserEndTime { get; set; }
        public int? TeamID { get; set; }
        public string TeamCity { get; set; }
        public string TeamName { get; set; }
    }
}
