using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace LibararyWebApplication.Pages.Student
{
    public class BorrowingHistoryModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public BorrowingHistoryModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public string? Message { get; set; }

        public List<BorrowingHistoryItem> Borrowings { get; set; } = new();

        public class BorrowingHistoryItem
        {
            public string BookTitle { get; set; } = "";
            public DateTime RentalDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Status { get; set; } = "";
            public int RenewCount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient();

            // Set base URL động
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            client.BaseAddress = new Uri(baseUrl);

            // Thêm token thủ công
            var token = GetTokenFromRequest();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }else{
                 // redirect to loginq
                 return Redirect("/login?return_url=...");
                //  Response.Redirect("/login");
            }

            try
            {
                var rentals = await client.GetFromJsonAsync<List<BorrowingHistoryItem>>("/api/borrowing/history/current");
                if (rentals != null)
                {
                    Borrowings = rentals;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Message = "You need to login";
                }
                else
                {
                    Message = $"Cant get book data {ex.Message}";
                }
            }

            return Page();
        }

        private string GetTokenFromRequest()
        {
            // Lấy token từ cookie
            var token = Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            // Lấy token từ Authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length);
            }

            return string.Empty;
        }
    }
}
