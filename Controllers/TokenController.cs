using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LibararyWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly TokenService token_service;
        private readonly PrnContext ctx;

        public TokenController(TokenService tokenService, PrnContext context)
        {
            token_service = tokenService;
            ctx = context;
        }

        public class AccessTokenRequest
        {
            public string username { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
        }

        [HttpPost("/api/getaccesstoken")]
        public IActionResult Post([FromBody] AccessTokenRequest model)
        {
            try
            {
                string username = model.username;
                string password = model.password;

                var user = ctx.Users.FirstOrDefault(u => (u.Username == username) && (u.Password == password) && u.Active);
                if (user != null)
                {
                    string access_token = token_service.GenerateToken(user);
                    return Ok(new
                    {
                        access_token = access_token
                    });

                    // JObject retval = new JObject();
                    // retval["access_token"] = access_token;
                    // return Ok(JsonConvert.SerializeObject(retval));

                    // cookie
                    // page login js send request -> set localstorage / set cookie
                    // page login 2 -> document.cookies.set('token', ...)
                    // localstorage -> .cs khong biet token
                    // cookies -> cs lay duoc token
                    // blazor client side / server side
                    // client side -> localstroage
                    // server side -> dung cookie
                    //
                    // js -> ajax -> token -> localstorage/cookie
                    //
                }

                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while generating the access token.",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
    }
}
