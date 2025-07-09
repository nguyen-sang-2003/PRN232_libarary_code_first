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

        public async Task<IActionResult> OnGetAsync()
        {
            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            var category =  await client.GetFromJsonAsync<List<Category>>($"{api_endpoint}/api/Categories");
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name");
            ViewData["Categories"] = new SelectList(category, "Id", "Name");
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
