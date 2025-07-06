using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.BookManagement
{
    public class IndexModel : PageModel
    {
        private readonly PrnContext _context;

        public IndexModel(PrnContext context)
        {
            _context = context;
        }

        public IList<Book> Book { get;set; } = default!;

        public async Task OnGetAsync()
        {
            using HttpClient client = new HttpClient();
          string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy

            Book = await client.GetFromJsonAsync<List<Book>>($"{api_endpoint}/api/Books");

        }
    }
}
