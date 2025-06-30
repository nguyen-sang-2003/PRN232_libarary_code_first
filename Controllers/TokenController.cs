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
        private readonly TokenService _tokenService;
        private readonly PrnContext ctx;

        public TokenController(TokenService tokenService, PrnContext context)
        {
            _tokenService = tokenService;
            ctx = context;
        }

        //[Route("/api/accesstoken")]
        //[HttpPost("/api/accesstoken")]
        //[Route("/api/accesstoken")]
        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginModel model)
        //{
        //    var user = ctx.Users.FirstOrDefault(u => (u.Username == model.Username) && (u.Password == model.Password) && u.Active);
        //    if (user != null)
        //    {
        //        var token = _tokenService.GenerateToken(model.Username, user.Role);
        //        return Ok(new { Token = token });
        //    }

        //    // Replace with actual user validation (e.g., database check)
        //    //if (model.Username == "testuser" && model.Password == "testpassword")
        //    //{
        //    //    var token = _tokenService.GenerateToken(model.Username);
        //    //    return Ok(new { Token = token });
        //    //}

        //    return Unauthorized("Invalid credentials");
        //}


        [HttpPost("/api/getaccesstoken")]
        public IActionResult Post()
        {
            byte[] buffer = new byte[4096];
            var task1 = Request.Body.ReadAsync(buffer, 0, buffer.Length);
            task1.Wait();
            string request_body = System.Text.Encoding.UTF8.GetString(buffer);

            var input_obj = JsonConvert.DeserializeObject(request_body);
            var dict1 = input_obj as JObject;
            string username = dict1.GetValue("username").ToString();
            string password = dict1.GetValue("password").ToString();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenService._jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var _jwtSettings = _tokenService._jwtSettings;

            if (username == "a" && password == "a")
            {

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                //string jwt_secret = Configuration.GetSection("JWT")["SecretKey"];
                //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_secret));

                //SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                    );

                JObject retval = new JObject();
                retval["access_token"] = new JwtSecurityTokenHandler().WriteToken(token);


                return Ok(JsonConvert.SerializeObject(retval));
                //return Ok(               {
                //access_token: access_token
                //}
                //);
            }
            else
            if (username == "b" && password == "a")
            {

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "User")
                };

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                    );
                JObject retval = new JObject();
                retval["access_token"] = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(JsonConvert.SerializeObject(retval));
            }
            else
            {
                var user = ctx.Users.FirstOrDefault(u => (u.Username == username) && (u.Password == password) && u.Active);
                if (user != null)
                {
                    var claims = new[]
                    {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                    var token = new JwtSecurityToken(
                        issuer: _jwtSettings.Issuer,
                        audience: _jwtSettings.Audience,
                        claims: claims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: creds
                        );
                    JObject retval = new JObject();
                    retval["access_token"] = new JwtSecurityTokenHandler().WriteToken(token);


                    return Ok(JsonConvert.SerializeObject(retval));
                }
            }

            return Unauthorized();


            //var x1 = new StreamReader(HttpContext.Request.Input)
            //HttpContent
            //Request.Body.Read()
        }
    }
}
