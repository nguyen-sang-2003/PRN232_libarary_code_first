using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibararyWebApplication.Pages.RuleManager
{
    public class DeleteModel : PageModel
    {
        private readonly PrnContext _context;
        private HttpClient httpClient = new HttpClient();

        public DeleteModel(PrnContext context, HttpClient httpClient)
        {
            _context = context;
            this.httpClient = httpClient;
        }

        [BindProperty]
        public Rule Rule { get; set; }
        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Rules";
        public string existing_token { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


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
                return Redirect($"/login?return_url={System.Web.HttpUtility.UrlEncode(HttpContext.Request.Path)}");
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

            var response = await httpClient.GetAsync($"{ApiBase}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            Rule = JsonConvert.DeserializeObject<Rule>(json);

            if (Rule == null)
            {
                return NotFound();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"{ApiBase}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "❌ Xóa nội quy thất bại.");
                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}
