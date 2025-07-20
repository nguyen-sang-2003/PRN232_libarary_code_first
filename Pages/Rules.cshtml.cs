using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace LibararyWebApplication.Pages
{
    public class RulesModel : PageModel
    {
        private readonly PrnContext _context;

        private HttpClient httpClient = new HttpClient();
        public RulesModel(PrnContext context, HttpClient httpClient)
        {
            _context = context;
            this.httpClient = httpClient;
        }
        public List<Rule> Rules { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";
            var response = await httpClient.GetAsync($"{api_endpoint}/api/Rules");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Rules = JsonConvert.DeserializeObject<List<Rule>>(json);
            }
            else
            {
                Console.WriteLine($"Lỗi khi gọi API: {response.StatusCode}");
            }
            return Page();
        }
    }
}
