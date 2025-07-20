using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace LibararyWebApplication.Pages.Librarian.CategoriesManagement
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
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";

            // Serialize the Category object to JSON content
            var jsonContent = new StringContent(JsonConvert.SerializeObject(Category), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{api_endpoint}/api/Categories", jsonContent);

            return RedirectToPage("./Index");
        }
    }
}
