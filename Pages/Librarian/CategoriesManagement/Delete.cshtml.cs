using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.CategoriesManagement
{
    public class DeleteModel : PageModel
    {
        private readonly PrnContext _context;

        public DeleteModel(PrnContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host}";

            var response = await client.DeleteAsync($"{api_endpoint}/api/Categories/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // optional: log error or show message
                return NotFound();
            }

            return RedirectToPage("./Index");
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            var response = await client.DeleteAsync($"{api_endpoint}/api/Categories/{id}");


            return RedirectToPage("./Index");
        }
    }
}
