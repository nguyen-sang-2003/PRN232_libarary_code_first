using Azure;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace LibararyWebApplication.Pages
{
    public class IndexModel : PageModel
    {
        public string list_books_response_json { get; set; }
        public string pretty_json(string input_json)
        {
            var parsedJson = JsonConvert.DeserializeObject(input_json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        public string token_cookie { get; set; }

        public string current_host { get; set; }

        public void OnGet()
        {
            string tmp_str;
            if (HttpContext.Request.Cookies.TryGetValue("token", out tmp_str))
            {
                token_cookie = tmp_str;
            }
            // server
            current_host = HttpContext.Request.Host.ToString();

            string existing_token = Request.Headers.Authorization;
            if (existing_token == null)
            {
                // redirect login page
            }
            else
            {
                HttpClient httpClient = new HttpClient();
                // api endpoint = "http://localhost:5138/"
                //string api_endpoint = "http://localhost:5138/";
                // blazor
                string api_endpoint = "http://googleapi.chatgpt/";
                //api_endpoint = H
                // blazor
                // razor
                api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";
                // page 
                var task1 = httpClient.GetAsync($"{api_endpoint}/api/SampleEnityBooks");
                // server
                // set model obj render html

                task1.Wait();
                var response = task1.Result; // TODO check status code
                var task2 = response.Content.ReadAsStringAsync();
                task2.Wait();
                var response_str = task2.Result;
                list_books_response_json = pretty_json(response_str);
            }
        }
    }
}
