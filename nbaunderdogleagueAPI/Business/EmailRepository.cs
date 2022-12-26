using nbaunderdogleagueAPI.Communications;

namespace nbaunderdogleagueAPI.Business
{
    public interface IEmailRepository
    {
        Task<SendGrid.Response> InviteUserToGroup(string email, string groupId);
    }

    public class EmailRepository : IEmailRepository
    {
        private readonly IEmailHelper _emailHelper;
        public EmailRepository(IEmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }
        public Task<SendGrid.Response> InviteUserToGroup(string email, string groupId)
        {
            return _emailHelper.InviteUserToGroup(email, groupId);
        }
    }
}
