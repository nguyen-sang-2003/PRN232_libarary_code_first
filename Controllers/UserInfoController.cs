using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace LibararyWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserInfoController : ControllerBase
    {
        [HttpGet]
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
    }
}
