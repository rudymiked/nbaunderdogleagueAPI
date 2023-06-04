namespace nbaunderdogleagueAPI.Models
{
    public class GroupInvitation
    {
        public Guid? InviteId { get; set; }
        public Guid GroupId { get; set; }
        public string Email { get; set; }
    }
}
