using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppController : ControllerBase
    {
        public AppController() { }

        [Authorize]
        [HttpGet("Start")]
        public ActionResult Start()
        {
            return Ok();
        }
    }
}