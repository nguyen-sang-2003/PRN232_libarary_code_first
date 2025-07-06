using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Pages.Test
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
            Book = await _context.Books
                .Include(b => b.Author).ToListAsync();
        }
    }
}
