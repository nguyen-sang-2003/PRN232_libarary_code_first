using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class DeleteModel : PageModel
    {
        private readonly PrnContext _context;

        public DeleteModel(PrnContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";
          var book = await client.GetFromJsonAsync<Book>($"{api_endpoint}/api/Books/{id}");
            if (book == null) { return NotFound(); }
            else {
                Book = book;
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient httpClient = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";
            var respone = await httpClient.DeleteAsync($"{api_endpoint}/api/Books/{id}");


            return RedirectToPage("./Index");
        }
    }
}
