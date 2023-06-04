using Microsoft.AspNetCore.Mvc;

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