using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibararyWebApplication.Pages.RuleManager
{
    public class CreateModel : PageModel
    {
        private readonly PrnContext _context;

        public CreateModel(PrnContext context)
        {
            _context = context;
        }
        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Rules";

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Rule Rule { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(TitleRuleExists(Rule.Title))
            {
                ModelState.AddModelError("Rule.Title", "❌ Tiêu đề nội quy đã tồn tại.");
                return Page();
            }
            Rule.CreatedAt = DateTime.Now;
            Rule.UpdatedAt = DateTime.Now;

            using var httpClient = new HttpClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(Rule), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ApiBase, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }
            return RedirectToPage("./Index");
        }
        private bool TitleRuleExists(string title)
        {
            return _context.Rules.Any(e => e.Title == title);
        }
    }
}
