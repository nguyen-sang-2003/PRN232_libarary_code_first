using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Web;

namespace LibararyWebApplication.Pages
{
    public class ProfileModel : PageModel
    {
        public User? User { get; set; }
        public string existing_token { get; set; }
        private HttpClient httpClient = new HttpClient();
        public ProfileModel(HttpClient _httpClient)
        {
            httpClient = _httpClient;
        }
        public async Task<IActionResult> OnGetAsync()
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

                var username = jwtToken.Claims.FirstOrDefault(c =>
                                    c.Type == ClaimTypes.Name || c.Type == JwtRegisteredClaimNames.Sub)
                                    ?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Redirect($"/login?return_url={System.Web.HttpUtility.UrlEncode(HttpContext.Request.Path)}");
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

                //int testUserId = 2;
                //var response = await httpClient.GetAsync($"{api_endpoint}/api/Users/{testUserId}");
                var response = await httpClient.GetAsync($"{api_endpoint}/api/Users/by-username/{username}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    User = JsonConvert.DeserializeObject<User>(json);
                }
                else
                {
                    Console.WriteLine($"Lỗi khi gọi API: {response.StatusCode}");
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
