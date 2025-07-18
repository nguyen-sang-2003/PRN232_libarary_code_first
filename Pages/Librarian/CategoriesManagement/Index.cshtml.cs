using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.CategoriesManagement
{
    public class IndexModel : PageModel
    {


        public IList<categoryDTO> Category { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            HttpClient client = new HttpClient();


            string existing_token;

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

            var username = jwtToken.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.Name || c.Type == JwtRegisteredClaimNames.Sub)
                                ?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Redirect("/login");
            }
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);
            //Code
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            Category = await client.GetFromJsonAsync<List<categoryDTO>>($"{api_endpoint}/api/Categories");
            return Page();
        }
    }
}
