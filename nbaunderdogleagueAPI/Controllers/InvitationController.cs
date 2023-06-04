using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvitationController(IInvitationService invitationService, IHttpContextAccessor httpContextAccessor)
        {
            _invitationService = invitationService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GenerateGroupJoinLink")]
        public ActionResult<string> GenerateGroupJoinLink(GroupInvitation groupInvitation)
        {
            return _invitationService.GenerateGroupJoinLink(groupInvitation);
        }

        [HttpPost("JoinRequest")]
        public ActionResult<string> JoinRequest(GroupInvitation groupInvitation)
        {
            return _invitationService.JoinRequest(groupInvitation);
        }
    }
}
