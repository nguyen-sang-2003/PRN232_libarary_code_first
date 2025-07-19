using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.WebRequestMethods;

namespace LibararyWebApplication.Pages.cuong
{

	public class BookDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string AuthorName { get; set; }
		public string ImageBase64 { get; set; } // Optional: Can convert to full data URL in frontend
		public DateTime PublishedDate { get; set; }
		public int TotalCopies { get; set; }
		public int AvailableCopies { get; set; }

	}
	public class BookDetailModel : PageModel
	{
		public BookDto book { get; set; }
		public async Task OnGet(int id)
		{
			try
			{
				var Http = new HttpClient();
				string api_endpoint = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToString()}";
				book = await Http.GetFromJsonAsync<BookDto>($"{api_endpoint}/api/cuong/books/{id}");

			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Failed to load book detail: {ex.Message}");
			}
		}
	}
}
