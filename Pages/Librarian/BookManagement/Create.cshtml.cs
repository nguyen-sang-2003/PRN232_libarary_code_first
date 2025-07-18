using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class CreateModel : PageModel
    {
        private readonly PrnContext _context;
       public HttpClient client = new HttpClient();
        public CreateModel(PrnContext context, HttpClient client)
        {
            _context = context;
            this.client = client;

        }
        public List<Category> AllCategories { get; set; }

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
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
            await LoadCategoriesAsync();
            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {


            using HttpClient client = new HttpClient();

            // Lấy base URL của API (bạn có thể hardcode hoặc cấu hình nếu cần)
            var apiEndpoint = $"http://{HttpContext.Request.Host}/api/Books";

            // Chuyển Book sang BookDTO
            var dto = new BookDTO   
            {
                Name = Book.Title,
                AuthorId = Book.AuthorId,
                Image = Book.ImageBase64,
                PublicDate = Book.PublishedDate,
                CategoryIds = SelectedCategoryIds
            };

            var response = await client.PostAsJsonAsync(apiEndpoint, dto);

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToPage("./Index");
            }
            await LoadCategoriesAsync();
            // Nếu gọi API lỗi, trả về thông báo lỗi
            ModelState.AddModelError(string.Empty, "Lỗi khi tạo sách qua API.");
            return Page();
        }
        private async Task LoadCategoriesAsync()
        {

           
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";
            var category = await client.GetFromJsonAsync<List<Category>>($"{api_endpoint}/api/Categories");

            AllCategories = category ?? new List<Category>();
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name");
            ViewData["Categories"] = new SelectList(AllCategories, "Id", "Name");
        }

    }


}
