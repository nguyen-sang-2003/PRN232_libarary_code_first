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
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{ApiBase}/Returns/rental-detail?rentailId={rentailId}");
                if (!response.IsSuccessStatusCode) return;

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Detail = null;
                    return;
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Detail = JsonConvert.DeserializeObject<DetailRentail>(json);
                }
                else
                {
                    Console.WriteLine($"Lỗi khi gọi API: {response.StatusCode}");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }

        }
        public async Task<IActionResult> OnPostRenewAsync(int RentalId)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"{ApiBase}/Rentals/renew-book?rentalId={RentalId}", null);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Detail = null;
                return RedirectToPage(new { rentailId = RentalId });
            }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Rental>(json);
            }
            else
            {
                Console.WriteLine($"Lỗi khi gọi API: {response.StatusCode}");
            }
            return RedirectToPage(new { rentailId = RentalId });
        }

    }
}
