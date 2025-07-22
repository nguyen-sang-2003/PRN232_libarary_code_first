using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.RentalManagement
{
    public class IndexModel : PageModel
    {
        private readonly PrnContext _context;

        public IndexModel(PrnContext context)
        {
            _context = context;
        }
        HttpClient client = new HttpClient();

        [BindProperty(SupportsGet =true)]
        public string Status {  get; set; }

        [BindProperty(SupportsGet = true)]
        public int CopiesID { get; set; }
        [BindProperty(SupportsGet = true)]

        public int Id { get; set; }
        public IList<RentalDTO> Rental { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string existing_token = Request.Headers.Authorization;
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

            string api_endpoint = $"http://{HttpContext.Request.Host}";

            var rentals = await client.GetFromJsonAsync<List<RentalDTO>>($"{api_endpoint}/api/Rentals");

            if (!string.IsNullOrEmpty(SearchUser))
            {
                Rental = rentals.Where(r => r.UserName != null &&
                                 r.UserName.Contains(SearchUser, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                Rental = rentals;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string? token = Request.Headers.Authorization;
            if (string.IsNullOrEmpty(token))
            {
                token = Request.Cookies["token"];
            }

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            var respone = await client.PutAsJsonAsync($"{api_endpoint}/api/Rentals/{Id}?Status={Status}", CopiesID);
            Rental = await client.GetFromJsonAsync<List<RentalDTO>>($"{api_endpoint}/api/Rentals");
            return Page();
        }
    }
}
