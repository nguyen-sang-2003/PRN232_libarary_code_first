using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class CreateModel : PageModel
    {
        private readonly PrnContext _context;
        private readonly HttpClient _httpClient;

        public CreateModel(PrnContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        public int NumberOfCopies { get; set; } = 1;

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<Category> AllCategories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetTokenFromRequest();
            if (token == null) return Redirect("/login");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = GetTokenFromRequest();
            if (token == null) return Redirect("/login");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var apiEndpoint = $"http://{HttpContext.Request.Host}/api/Books";

            var dto = new BookDTO
            {
                Name = Book.Title,
                AuthorId = Book.AuthorId,
                Image = Book.ImageBase64,
                PublicDate = Book.PublishedDate,
                CategoryIds = SelectedCategoryIds,
                NumberOfCopies = this.NumberOfCopies // ➕ Thêm dòng này
            };

            var response = await _httpClient.PostAsJsonAsync(apiEndpoint, dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Lỗi khi tạo sách qua API.");
            await LoadCategoriesAsync();
            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var apiBase = $"http://{HttpContext.Request.Host}";
            AllCategories = await _httpClient.GetFromJsonAsync<List<Category>>($"{apiBase}/api/Categories") ?? new List<Category>();

            var authors = await _context.Authors.ToListAsync();
            ViewData["AuthorId"] = new SelectList(authors, "Id", "Name");
        }

        private string? GetTokenFromRequest()
        {
            string? token = Request.Headers.Authorization;
            if (string.IsNullOrEmpty(token))
                token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token)) return null;

            if (token.StartsWith("Bearer "))
                token = token.Substring("Bearer ".Length);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var username = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Name || c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return string.IsNullOrEmpty(username) ? null : token;
        }
    }
}
