using System;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;

namespace rapid_moose.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private const string apikey_var = "API_KEY";
        private static readonly string apiKey = Environment.GetEnvironmentVariable(apikey_var);

        [HttpGet("chapterset")]
        public ActionResult<string> GetChapterSet([FromQuery] string book, [FromHeader] int ApiAccessKey)
        {
            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            if (string.IsNullOrEmpty(book) | book.Contains("'") | book.Contains(")"))
            {
                return BadRequest();
            }
            return (Data.GetChapters(book));
        }

        [HttpGet("wordset")]
        public ActionResult<string> GetWordSet([FromQuery] string book, [FromQuery] int chapter, [FromHeader] int ApiAccessKey)
        {
            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            if (string.IsNullOrEmpty(book) | book.Contains("'") | book.Contains(")"))
            {
                return BadRequest();
            }
            return (Data.GetWords(book,chapter));
        }
    }
}