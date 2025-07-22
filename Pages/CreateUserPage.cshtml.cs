using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

namespace LibararyWebApplication.Pages
{
    public class CreateUserPageModel : PageModel
    {
        private readonly PrnContext _context;
        public string existing_token { get; set; }
        private HttpClient httpClient = new HttpClient();
        public CreateUserPageModel(PrnContext context, HttpClient client)
        {
            _context = context;
            httpClient = client;
        }

        [BindProperty]
        public User user { get; set; }
        public async Task<ActionResult> OnGet()
        {
            existing_token = Request.Headers.Authorization;
            if (existing_token == null)
            {
                existing_token = Request.Cookies["token"];
            }
            if (existing_token == null)
            {
                return Redirect($"/login?return_url={System.Web.HttpUtility.UrlEncode(HttpContext.Request.Path)}");
            }

            if (existing_token.StartsWith("Bearer "))
            {
                existing_token = existing_token.Substring("Bearer ".Length);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(existing_token);

            var role = jwtToken.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.Role || c.Type == JwtRegisteredClaimNames.Jti)
                                ?.Value;

            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                return Unauthorized();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";

                string current_host = HttpContext.Request.Host.ToString();

                existing_token = Request.Headers.Authorization;
                if (existing_token == null)
                {
                    existing_token = Request.Cookies["token"];
                }
                if (existing_token == null)
                {
                    return Redirect($"/login?return_url={System.Web.HttpUtility.UrlEncode(HttpContext.Request.Path)}");
                }

                // Xử lý nếu token có dạng "Bearer xxx"
                if (existing_token.StartsWith("Bearer "))
                {
                    existing_token = existing_token.Substring("Bearer ".Length);
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(existing_token);

                var role = jwtToken.Claims.FirstOrDefault(c =>
                                    c.Type == ClaimTypes.Role || c.Type == JwtRegisteredClaimNames.Jti)
                                    ?.Value;

                if (string.IsNullOrEmpty(role) || role != "admin")
                {
                    return Unauthorized();
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

                var encodedEmail = System.Web.HttpUtility.UrlEncode(user.Email);
                var response = await httpClient.GetAsync($"{api_endpoint}/api/Users/by-email/{encodedEmail}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    User userExit = JsonConvert.DeserializeObject<User>(json);
                    if (userExit != null)
                    {
                        TempData["ErrorMessage"] = "Email already exists.";
                        return Page();
                    }
                }
                else
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response1 = await httpClient.PostAsync($"{api_endpoint}/api/Users", jsonContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("/UserAdmin");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            return Page();
        }
    }
}
