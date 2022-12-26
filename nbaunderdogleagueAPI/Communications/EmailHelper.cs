using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace nbaunderdogleagueAPI.Communications
{
    public interface IEmailHelper
    {
        Task<Response> InviteUserToGroup(string email, string groupId);
    }
    public class EmailHelper: IEmailHelper
    {
        AppConfig _appConfig;
        ILogger _logger;
        IGroupService _groupService;

        public EmailHelper(IOptions<AppConfig> appConfig, ILogger<NBADataAccess> logger, IGroupService groupService)
        {
            _appConfig = appConfig.Value;
            _logger = logger;
            _groupService = groupService;
        }

        public async Task<Response> InviteUserToGroup(string email, string groupId)
        {
            var client = new SendGridClient(_appConfig.nbaunderdogleagueSendGridKey);

            GroupEntity group = _groupService.GetGroup(groupId);

            EmailAddress from = new(_appConfig.NBAEmail, AppConstants.SiteTitle);
            EmailAddress to = new(email, email.Split('@')[0]);

            string subject = "NBA Fantasy Invitation - NBA Underdogs";
            string plainTextContent = "You're invited!";
            string body = "\r\nYou're invited to play NBA Fantasy with NBA Underdogs!\r\n<br /><br />" +
                "Click the link below to join <strong>" + group.Name + "</strong>:\r\n<br /><br />";

            string headerImage = "<img src='https://nbaunderdogleaguestorage.blob.core.windows.net/images/nbafantasy_right.png' />";

            string joinURL = "https://www.w3docs.com/";

            string joinButton = "<form action=\"" + joinURL + "\" method=\"get\" target=\"_blank\">\r\n" +
                                    "<button type=\"submit\">Join League</button>\r\n" +
                                "</form>\r\n<br /><br />\r\n";

            string style = "<style>\r\n      " +
                                ".button {\r\n        " +
                                "display: inline-block;\r\n        " +
                                "padding: 10px 20px;\r\n        " +
                                "text-align: center;\r\n        " +
                                "text-decoration: none;\r\n        " +
                                "color: #ffffff;\r\n        " +
                                "background-color: #bf4c26;\r\n        " +
                                "border-radius: 6px;\r\n        " +
                                "outline: none;\r\n      }\r\n    " +
                            "</style>";

            string htmlContent = "<!DOCTYPE html>\r\n<html>\r\n  " +
                "<head>\r\n" +
                "<title>" + subject + "</title>\r\n  " + style +
                "</head>\r\n" +
                "<body>\r\n" + body + joinButton +
                "</body>\r\n" + 
                "</html>";


            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            return await client.SendEmailAsync(msg);
        }
    }
}
