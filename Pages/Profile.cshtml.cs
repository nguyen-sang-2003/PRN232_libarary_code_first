using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;

namespace LibararyWebApplication.Pages
{
    public class ProfileModel : PageModel
    { 
        public User? User { get; set; }
        public string existing_token { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";

                string current_host = HttpContext.Request.Host.ToString();

                 existing_token = Request.Headers.Authorization;
                if (existing_token == null)
                {
                    Redirect($"http://{current_host}/login");
                }

                int testUserId = 1;

                var response = await httpClient.GetAsync($"{api_endpoint}/api/Users/{testUserId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    User = JsonConvert.DeserializeObject<User>(json);
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
    }
}
