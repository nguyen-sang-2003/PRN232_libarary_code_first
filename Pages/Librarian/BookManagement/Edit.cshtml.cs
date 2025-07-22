using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
    public class EditModel : PageModel
    {
        private readonly PrnContext _context;
        public HttpClient client = new HttpClient();
        public EditModel(PrnContext context, HttpClient client)
        {
            _context = context;
            this.client = client;
        }
        [BindProperty]
        public int NumberOfCopies { get; set; } = 0;

        public int CurrentCopies { get; set; } = 0;

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<Category> AllCategories { get; set; } = new List<Category>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {



            // Code
            if (id == null) return NotFound();

            // Load book cùng categories
            Book = await _context.Books
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Book == null) return NotFound();

            // Gán SelectedCategoryIds từ book
            SelectedCategoryIds = Book.Categories.Select(c => c.Id).ToList();
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
            // Load category & author
            await LoadCategoriesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {


            // Chuẩn bị dữ liệu
            var bookDto = new BookDTO
            {
                Name = Book.Title,
                AuthorId = Book.AuthorId,
                Image = Book.ImageBase64,
                PublicDate = Book.PublishedDate,
                CategoryIds = SelectedCategoryIds
            };

            using HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host}";

            // Gọi API cập nhật thông tin sách
            var response = await client.PutAsJsonAsync($"{api_endpoint}/api/Books/{Book.Id}", bookDto);

            if (response.IsSuccessStatusCode)
            {
                // ➕ Thêm copies mới nếu cần
                if (NumberOfCopies > 0)
                {
                    for (int i = 0; i < NumberOfCopies; i++)
                    {
                        var copy = new BookCopy
                        {
                            BookId = Book.Id,
                            Status = "available",
                            Condition = "new",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.BookCopies.Add(copy);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("./Index");
            }


            ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật sách.");

            return Page();
        }

        private async Task LoadCategoriesAsync()
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

            string api_endpoint = $"http://{HttpContext.Request.Host}";

            AllCategories = await client.GetFromJsonAsync<List<Category>>($"{api_endpoint}/api/Categories")
                              ?? new List<Category>();

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name");
            ViewData["Categories"] = new SelectList(AllCategories, "Id", "Name");
        }

    }
}
