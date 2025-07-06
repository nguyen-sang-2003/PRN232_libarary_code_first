using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class CreateModel : PageModel
    {
        private readonly PrnContext _context;

        public CreateModel(PrnContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id");
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
                PublicDate = Book.PublishedDate
            };

            var response = await client.PostAsJsonAsync(apiEndpoint, dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            // Nếu gọi API lỗi, trả về thông báo lỗi
            ModelState.AddModelError(string.Empty, "Lỗi khi tạo sách qua API.");
            return Page();
        }
    }

}
