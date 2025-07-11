using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.CategoriesManagement
{
    public class IndexModel : PageModel
    {


        public IList<Category> Category { get;set; } = default!;

        public async Task OnGetAsync()
        {
            HttpClient client = new HttpClient();
            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            Category = await client.GetFromJsonAsync<List<Category>>($"{api_endpoint}/api/Categories");
        }
    }
}
