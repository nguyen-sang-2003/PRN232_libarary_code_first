using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.CategoriesManagement
{
    public class EditModel : PageModel
    {
        private readonly PrnContext _context;

        public EditModel(PrnContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
           HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            if (id == null)
            {
                return NotFound();
            }
            var category = await client.GetFromJsonAsync<Category>($"{api_endpoint}/api/Categories/{id}");
            Category = category;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            //var respone = await client.PutAsync($"{api_endpoint}/api/Categories/{Category.Id}?Name={Category.Name}",null);
            var response = await client.PutAsJsonAsync<Category>($"{api_endpoint}/api/Categories/{Category.Id}?Name={Category.Name}", Category);
            return RedirectToPage("./Index");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
