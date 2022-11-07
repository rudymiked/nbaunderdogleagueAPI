using Microsoft.AspNetCore.Mvc;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppController : ControllerBase
    {

        public AppController() { }

        [HttpGet("Start")]
        public ActionResult Start(bool start)
        {
            return start ? Ok() : NoContent();
        }
    }
}