namespace nbaunderdogleagueAPI.Models
{
    public class Scoreboard
    {
        public string HomeGovernor { get; set; }
        public string HomeLogo { get; set; }
        public string HomeTeam { get; set; }
        public int? HomeScore { get; set; }
        public string VisitorsGovernor { get; set; }
        public string VisitorsLogo { get; set; }
        public string VisitorsTeam { get; set; }
        public int? VisitorsScore { get; set; }
        public DateTimeOffset GameDate { get; set; }
    }
}
