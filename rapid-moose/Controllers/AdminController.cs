using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace rapid_moose.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        [Route("users")]
        public ActionResult<string> GetUsers([FromHeader] int ApiAccessKey, [FromHeader] int ApiAdminKey)
        {
            int adminKey = Convert.ToInt32(Environment.GetEnvironmentVariable("ADMIN_KEY"));

            if (!Session.CheckSession(ApiAccessKey) | ApiAdminKey != adminKey)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return Admin.GetUsers();
        }

        [HttpGet]
        [Route("sessions")]
        public ActionResult<string> GetSessions([FromHeader] int ApiAccessKey, [FromHeader] int ApiAdminKey)
        {
            int adminKey = Convert.ToInt32(Environment.GetEnvironmentVariable("ADMIN_KEY"));

            if (!Session.CheckSession(ApiAccessKey) | ApiAdminKey != adminKey)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return Admin.GetSessions();
        }
    }
}
