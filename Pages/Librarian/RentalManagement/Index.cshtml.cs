using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Librarian.RentalManagement
{
    public class IndexModel : PageModel
    {
        private readonly PrnContext _context;

        public IndexModel(PrnContext context)
        {
            _context = context;
        }
        HttpClient client = new HttpClient();

        [BindProperty(SupportsGet =true)]
        public string Status {  get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public IList<RentalDTO> Rental { get;set; } = default!;

        public async Task OnGetAsync()
        {

            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            Rental = await client.GetFromJsonAsync<List<RentalDTO>>($"{api_endpoint}/api/Rentals");

        }
        public async Task OnPostAsync()
        {

            string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}"; // đổi theo địa chỉ backend bạn chạy
            var respone = await client.PutAsync($"{api_endpoint}/api/Rentals/{Id}?Status={Status}",null);
            Rental = await client.GetFromJsonAsync<List<RentalDTO>>($"{api_endpoint}/api/Rentals");
        }
    }
}
