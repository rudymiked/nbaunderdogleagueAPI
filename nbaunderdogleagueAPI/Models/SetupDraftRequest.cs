namespace nbaunderdogleagueAPI.Models
{
    public class SetupDraftRequest
    {
        public string GroupId { get; set; }
        public string Email { get; set; }
        public bool ClearTeams { get; set; }
        public DateTime DraftStartDateTime { get; set; }
        public int DraftWindow { get; set; }
    }
}
