using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace LibararyWebApplication.Pages
{
    public class UserAdminModel : PageModel
    {
        public List<User> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "Page")]
        public int? Page { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int CurrentPage;

        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Users";
        public string existing_token { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //Console.WriteLine($"[LOG] SearchTerm = '{SearchTerm}', RoleFilter = '{RoleFilter}', Page = {Page}");
            using var httpClient = new HttpClient();

            existing_token = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(existing_token))
            {
                existing_token = Request.Cookies["token"];
            }

            if (string.IsNullOrEmpty(existing_token))
            {
                return Redirect("/login");
            }

            if (existing_token.StartsWith("Bearer "))
            {
                existing_token = existing_token.Substring("Bearer ".Length);
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", existing_token);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(existing_token);
            var role = jwtToken.Claims.FirstOrDefault(c =>
                                    c.Type == ClaimTypes.Role || c.Type == JwtRegisteredClaimNames.Jti)
                                    ?.Value;
            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                return Redirect("/login");
            }

            var response = await httpClient.GetAsync(ApiBase);
            if (!response.IsSuccessStatusCode) return Page();

            var json = await response.Content.ReadAsStringAsync();
            var allUsers = JsonConvert.DeserializeObject<List<User>>(json) ?? new();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                allUsers = allUsers.Where(u => u.Username.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(RoleFilter))
            {
                allUsers = allUsers.Where(u => u.Role == RoleFilter).ToList();
            }

            int currentPage = (Page != null && Page > 0) ? Page.Value : 1;

            TotalPages = (int)Math.Ceiling(allUsers.Count / (double)PageSize);
            CurrentPage = currentPage;

            Users = allUsers
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            using var httpClient = new HttpClient();

            existing_token = Request.Cookies["token"];
            if (!string.IsNullOrEmpty(existing_token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", existing_token);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(existing_token);
            var role = jwtToken.Claims.FirstOrDefault(c =>
                                    c.Type == ClaimTypes.Role || c.Type == JwtRegisteredClaimNames.Jti)
                                    ?.Value;
            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                return Redirect("/login");
            }

            var response = await httpClient.DeleteAsync($"{ApiBase}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }
            return RedirectToPage("UserAdmin");
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int id, string newRole)
        {
            using var httpClient = new HttpClient();

            existing_token = Request.Cookies["token"];
            if (!string.IsNullOrEmpty(existing_token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", existing_token);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(existing_token);
            var role = jwtToken.Claims.FirstOrDefault(c =>
                                    c.Type == ClaimTypes.Role || c.Type == JwtRegisteredClaimNames.Jti)
                                    ?.Value;
            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                return Redirect("/login");
            }

            var content = new StringContent(JsonConvert.SerializeObject(new { Role = newRole }), Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{ApiBase}/{id}/role", content);
            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }
            return RedirectToPage("UserAdmin");
        }
    }
}
