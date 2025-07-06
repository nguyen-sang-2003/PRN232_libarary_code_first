using Microsoft.AspNetCore.Authorization;
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
                if (existing_token == null)// remove bearer
                {
                    // get token from cookie
                    existing_token = Request.Cookies["token"];
                }
                if (existing_token == null)
                {
                    Redirect($"http://{current_host}/login");
                }

                // set header for httpClient
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existing_token);
                var json_user_info = await httpClient.GetStringAsync(
                    $"{api_endpoint}/api/users/info");
                var user_info = JsonConvert.DeserializeObject<dynamic>(json_user_info);
                System.Console.WriteLine($"User info: {json_user_info}");

                if (user_info == null || user_info.Name == null)
                {
                    // Redirect to login if user info is not available
                    var returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                    Redirect($"/login?return_url={Uri.EscapeDataString(returnUrl)}");
                }

                string user_name = user_info.Name.ToString();
                // Check if the user exists in the database
                var ctx = new PrnContext();
                var user_obj = ctx.Users.FirstOrDefault(u => u.Username == user_name);
                if (user_obj == null)
                {
                    // User not found, redirect to login
                    var returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                    Redirect($"/login?return_url={Uri.EscapeDataString(returnUrl)}");
                }
                else
                {
                    User = user_obj;
                }

                // int testUserId = 1;

                // var response = await httpClient.GetAsync($"{api_endpoint}/api/Users/{testUserId}");

                // if (response.IsSuccessStatusCode)
                // {
                //     var json = await response.Content.ReadAsStringAsync();
                //     User = JsonConvert.DeserializeObject<User>(json);
                // }
                // else
                // {
                //     Console.WriteLine($"Lỗi khi gọi API: {response.StatusCode}");
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
}
