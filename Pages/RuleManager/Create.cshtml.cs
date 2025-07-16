using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibararyWebApplication.Pages.RuleManager
{
    public class CreateModel : PageModel
    {
        private readonly PrnContext _context;
        public string existing_token { get; set; }
        private HttpClient httpClient = new HttpClient();

        public CreateModel(PrnContext context, HttpClient httpClient)
        {
            _context = context;
            this.httpClient = httpClient;
        }
        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Rules";

        public IActionResult OnGet()
        {
            existing_token = Request.Headers.Authorization;
            if (existing_token == null)
            {
                existing_token = Request.Cookies["token"];
            }
            if (existing_token == null)
            {
                return Redirect($"/login?return_url={HttpUtility.UrlEncode(HttpContext.Request.Path)}");
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
                return Redirect($"/login?return_url={HttpUtility.UrlEncode(HttpContext.Request.Path)}");
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

            return Page();
        }

        [BindProperty]
        public Rule Rule { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(TitleRuleExists(Rule.Title))
            {
                ModelState.AddModelError("Rule.Title", "❌ Tiêu đề nội quy đã tồn tại.");
                return Page();
            }
            Rule.CreatedAt = DateTime.Now;
            Rule.UpdatedAt = DateTime.Now;

            using var httpClient = new HttpClient();

            existing_token = Request.Headers.Authorization;
            if (existing_token == null)
            {
                existing_token = Request.Cookies["token"];
            }
            if (existing_token == null)
            {
                return Redirect($"/login?return_url={HttpUtility.UrlEncode(HttpContext.Request.Path)}");
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
                return Redirect($"/login?return_url={HttpUtility.UrlEncode(HttpContext.Request.Path)}");
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(Rule), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ApiBase, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }
            return RedirectToPage("./Index");
        }
        private bool TitleRuleExists(string title)
        {
            return _context.Rules.Any(e => e.Title == title);
        }
    }
}
