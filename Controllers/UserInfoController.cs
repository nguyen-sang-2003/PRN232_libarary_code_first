using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace LibararyWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly PrnContext ctx;
        public UserInfoController(PrnContext context)
        {
            ctx = context;
        }

        [HttpGet("/api/users/info")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var user = HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userInfo = new
            {
                Name = user.Identity.Name,
                Claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };

            return Ok(userInfo);
        }

        public class UserRegistrationModel
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        [HttpPost("/api/users/register")]
        public IActionResult RegisterUser([FromBody] UserRegistrationModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.username) || string.IsNullOrEmpty(model.password))
                {
                    return BadRequest("Invalid registration data.");
                }

                // Check if the user already exists
                var existingUser = ctx.Users.FirstOrDefault(u => u.Username == model.username);
                if (existingUser != null)
                {
                    return Conflict("User already exists.");
                }

                // Create a new user
                var newUser = new User
                {
                    Username = model.username,
                    Password = model.password, // In a real application, ensure to hash the password
                    Active = true,
                    Role = "user" // Default role, adjust as necessary
                };

                ctx.Users.Add(newUser);
                ctx.SaveChanges();

                return Ok(new { Message = "User registered successfully." });

            } catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while registering the user.",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
    }
}
