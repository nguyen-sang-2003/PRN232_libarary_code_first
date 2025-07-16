using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibararyWebApplication.Pages.RuleManager
{
    public class IndexModel : PageModel
    {
        private readonly PrnContext _context;

        private HttpClient httpClient = new HttpClient();
        public IndexModel(PrnContext context, HttpClient httpClient)
        {
            _context = context;
            this.httpClient = httpClient;
        }

        public IList<Rule> Rule { get; set; } = new List<Rule>();

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        private const int PageSize = 5;
        public int TotalPages { get; set; }

        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Rules";
        public string existing_token { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {


            existing_token = Request.Headers.Authorization;
            if (existing_token == null)
            {
                existing_token = Request.Cookies["token"];
            }
            if (existing_token == null)
            {
                return Redirect("/login");
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
                return Redirect("/login");
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

            var response = await httpClient.GetAsync(ApiBase);
            if (!response.IsSuccessStatusCode) return Page();

            var json = await response.Content.ReadAsStringAsync();
            var query = JsonConvert.DeserializeObject<List<Rule>>(json) ?? new();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(r => r.Title.Contains(SearchKeyword) || r.Content.Contains(SearchKeyword)).ToList();
            }

            int totalCount = query.Count;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            Rule = query
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return Page();
        }
    }
}
