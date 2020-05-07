using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Security.Cryptography;

namespace rapid_moose.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {     
        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            if(user == null)
            {
                return BadRequest();
            }

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
            try
            {
                bool unused = rapid_moose.User.CheckEmailUnused(email);
                if (unused)
                {
                    rapid_moose.User.CreateUser(email, hashedPassword);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}
