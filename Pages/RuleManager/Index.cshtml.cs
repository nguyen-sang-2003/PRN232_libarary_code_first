using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LibararyWebApplication.Pages.RuleManager
{
    public class IndexModel : PageModel
    {
        private readonly PrnContext _context;


        public IndexModel(PrnContext context)
        {
            _context = context;
        }

        public IList<Rule> Rule { get; set; } = new List<Rule>();

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        private const int PageSize = 5;
        public int TotalPages { get; set; }

        private string ApiBase => $"http://{HttpContext.Request.Host}/api/Rules";
        public async Task OnGetAsync()
        {

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(ApiBase);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var query = JsonConvert.DeserializeObject<List<Rule>>(json) ?? new();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(r => r.Title.Contains(SearchKeyword) || r.Content.Contains(SearchKeyword)).ToList();
            }

            int totalCount = query.Count;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            Rule = query
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
