using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibararyWebApplication.Pages
{
    public class DetailRetailModel : PageModel
    {
        [TempData]
        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int rentailId { get; set; }

        public DetailRentail? Detail { get; set; }
        private string ApiBase => $"http://{HttpContext.Request.Host}";
        public string existing_token { get; set; }
        private HttpClient httpClient = new HttpClient();
        public DetailRetailModel(HttpClient _httpClient) {
            httpClient = _httpClient;
        }
        public async Task<ActionResult>  OnGetAsync()
        {
            try
            {
                

                existing_token = Request.Headers.Authorization;
                if (existing_token == null)
                {
                    existing_token = Request.Cookies["token"];
                }
                if (existing_token == null)
                {
                    return Redirect($"/login?return_url={HttpUtility.UrlEncode(HttpContext.Request.Path)}");
                }

                if (existing_token.StartsWith("Bearer "))
                {
                    existing_token = existing_token.Substring("Bearer ".Length);
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(existing_token);

                var username = jwtToken.Claims.FirstOrDefault(c =>
                                    c.Type == ClaimTypes.Name || c.Type == JwtRegisteredClaimNames.Sub)
                                    ?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Redirect($"/login?return_url={HttpUtility.UrlEncode(HttpContext.Request.Path)}");
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);

                var response = await httpClient.GetAsync($"{ApiBase}/api/Returns/rental-detail?rentailId={rentailId}");
                if (!response.IsSuccessStatusCode) return Page();

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Detail = null;
                    return Page();
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
            return Page();
        }
        public async Task<IActionResult> OnPostRenewAsync(int RentalId)
        {
            //using var httpClient = new HttpClient();

            var response1 = await httpClient.GetAsync($"{ApiBase}/api/Returns/rental-detail?rentailId={rentailId}");
            if (!response1.IsSuccessStatusCode) return Page();

            if (response1.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Detail = null;
                return Page();
            }

            if (response1.IsSuccessStatusCode)
            {
                var json = await response1.Content.ReadAsStringAsync();
                Detail = JsonConvert.DeserializeObject<DetailRentail>(json);
            }
            else
            {
                Console.WriteLine($"Lỗi khi gọi API: {response1.StatusCode}");
            }

            if (Detail != null && Detail.RenewCount >= 3)
            {
                ErrorMessage = "Bạn không thể gia hạn thêm lần nào nữa (tối đa 3 lần).";
                return RedirectToPage(new { rentailId = RentalId });
            }

            string api_url = $"{ApiBase}/api/users/rental/renew-book?rentalId={RentalId}";
            Console.WriteLine(api_url);
            var response = await httpClient.PostAsync(api_url, null);

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
