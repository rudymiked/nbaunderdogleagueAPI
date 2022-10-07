namespace nbaunderdogleagueAPI.Models
{
    public class AppConfig
    {
        public string TableConnection { get; set; }
        public string apiAppId { get; set; }
        public string apiAppSecret { get; set; }
        public string NBAAppInsights { get; set; }
        public int DraftStartHour { get; set; }
        public int DraftStartDay { get; set; }
        public int DraftStartMonth { get; set; }
        public int DraftStartMinute { get; set; }
        public int DraftWindowMinutes { get; set; }
        public int MaxGroupsPerOwner { get; set; }
        public int TestDraft { get; set; }
    }
}
