namespace nbaunderdogleagueAPI.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? TeamCity { get; set; }
        public int ProjectedWin { get; set; }
        public int ProjectedLoss { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
        public string? Playoffs { get; set; }
    }
}
