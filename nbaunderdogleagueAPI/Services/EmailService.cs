using nbaunderdogleagueAPI.Business;

namespace nbaunderdogleagueAPI.Services
{
    public interface IEmailService
    {
        Task<SendGrid.Response> InviteUserToGroup(string email, string groupId);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }
        public Task<SendGrid.Response> InviteUserToGroup(string email, string groupId)
        {
            return _emailRepository.InviteUserToGroup(email, groupId);
        }
    }
}
