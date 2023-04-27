namespace nbaunderdogleagueAPI.Models
{
    public class ApproveUserRequest
    {
        public string GroupId { get; set; }
        public string InviteId { get; set; }
        public string Email { get; set; }
        public string AdminEmail { get; set; }
    }
}
