namespace nbaunderdogleagueAPI.Models
{
    public class AppConfig
    {
        public string TableConnection { get; set; }
        public string apiAppId { get; set; }
        public string apiAppSecret { get; set; }
        public string NBAAppInsights { get; set; }
        public int MaxGroupsPerOwner { get; set; }
        public int TestDraft { get; set; }
    }
}
