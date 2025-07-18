using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class DeleteModel : PageModel
    {
        private readonly PrnContext _context;

        public DeleteModel(PrnContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            using var httpClient = new HttpClient();
            string apiEndpoint = $"http://{HttpContext.Request.Host}";

            var response = await httpClient.DeleteAsync($"{apiEndpoint}/api/Books/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // Có thể log lỗi hoặc hiển thị thông báo tại đây nếu cần
                return BadRequest("Failed to delete the book.");
            }

            return RedirectToPage("./Index");
        }
    }
}
