using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibararyWebApplication.Pages.cuong
{

	public class CategoryDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	public class HomeModel : PageModel
	{
		private readonly HttpClient client;
		public HomeModel(HttpClient client)
		{
			this.client = client;
		}
		public List<BookDto> books { get; set; }
		public List<CategoryDto> categories { get; set; }
		public async Task OnGet()
		{

			try
			{
				string api_endpoint = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToString()}";
				Console.WriteLine(api_endpoint);
				books = await client.GetFromJsonAsync<List<BookDto>>($"{api_endpoint}/api/cuong/books");
				categories = await client.GetFromJsonAsync<List<CategoryDto>>($"{api_endpoint}/api/cuong/categories");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Failed to fetch data: {ex.Message}");
				books = null;
				categories = null;
			}
		}
	}
}
