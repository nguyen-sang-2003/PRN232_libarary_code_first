using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace LibararyWebApplication.Pages
{
    public class DetailRetailModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int rentailId { get; set; }

        public DetailRentail? Detail { get; set; }
        private string ApiBase => $"http://{HttpContext.Request.Host}/api";
        public async Task OnGetAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{ApiBase}/Returns/rentailId?rentailId={rentailId}");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            Detail = JsonConvert.DeserializeObject<DetailRentail>(json);




        }
    }
}
