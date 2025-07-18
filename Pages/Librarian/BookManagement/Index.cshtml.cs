using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class IndexModel : PageModel
    {
        private readonly PrnContext _context;
        private readonly HttpClient _httpClient;

        public IndexModel(PrnContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;

        }

        public IList<BookViewDTO> Book { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
          
            string existing_token;

            existing_token = Request.Headers.Authorization;
            if (existing_token == null)
            {
                existing_token = Request.Cookies["token"];
            }
            if (existing_token == null)
            {
               return  Redirect("/login");
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

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);
            //using HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy

           
            var Rental = await _httpClient.GetFromJsonAsync<List<BookViewDTO>>($"{api_endpoint}/api/Books");
            Book = Rental;
            return Page();
        }
        
    }
}
