using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using System.Text;

namespace rapid_moose.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get([FromHeader] int ApiAccessKey)
        {
            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            int userId = Session.LookUpUserId(ApiAccessKey);

            if (userId == -1)
            {
                return BadRequest();
            }

            string data = await rapid_moose.File.DownloadUserFile(userId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromHeader] int ApiAccessKey)
        {
            string data = "";
            HttpRequest req = Request;

            using (StreamReader reader
                      = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                data = await reader.ReadToEndAsync();
            }

            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }

            int userId = Session.LookUpUserId(ApiAccessKey);

            if(userId == -1)
            {
                return BadRequest();
            }

            rapid_moose.File.UploadUserFile(userId, data);

            return Ok();
        }
    }
}
