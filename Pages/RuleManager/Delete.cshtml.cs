using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibararyWebApplication.Pages.RuleManager
{
    public class DeleteModel : PageModel
    {
        private readonly PrnContext _context;

        public DeleteModel(PrnContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rule Rule { get; set; }
        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Rules";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{ApiBase}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            Rule = JsonConvert.DeserializeObject<Rule>(json);

            if (Rule == null)
            {
                return NotFound();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"{ApiBase}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "❌ Xóa nội quy thất bại.");
                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}
