using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace LibararyWebApplication.Pages.Student
{
    public class BorrowingHistoryModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public BorrowingHistoryModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }

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

        public async Task OnGetAsync(int userId = 1)
        {
            UserId = userId;
            var client = _clientFactory.CreateClient("BackendApi");

            if (UserId <= 0)
            {
                Message = "UserId không hợp lệ.";
                return;
            }

            try
            {
                var rentals = await client.GetFromJsonAsync<List<BorrowingHistoryItem>>($"/api/borrowing/history/user/{UserId}");
                if (rentals != null)
                {
                    Borrowings = rentals;
                }
            }
            catch (HttpRequestException ex)
            {
                Message = $"Không lấy được lịch sử mượn sách: {ex.Message}";
            }
        }
    }
}
