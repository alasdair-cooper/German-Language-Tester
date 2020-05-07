using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Security.Cryptography;
using System.Text;

namespace rapid_moose.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        [HttpPost]
        public ActionResult<int> Post([FromBody] User user)
        {
            string email = user.email;
            string password = user.password;

            if (email.Contains("'") | email.Contains(")") | password.Contains("'") | password.Contains(")"))
            {
                return BadRequest();
            }

            string hashedPassword = "";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            int userId = rapid_moose.User.CheckUser(email, hashedPassword);
            if (userId != -1)
            {
                return Ok(Session.CreateSession(userId, HttpContext.Connection.RemoteIpAddress.ToString()));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            Session.ExpireSessions();

            return Ok();
        }
    }
}
