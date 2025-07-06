    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    namespace LibararyWebApplication.Pages.Librarian.BookManagement
    {
        public class EditModel : PageModel
        {
            private readonly PrnContext _context;

            public EditModel(PrnContext context)
            {
                _context = context;
            }

            [BindProperty]
            public Book Book { get; set; } = default!;

            public async Task<IActionResult> OnGetAsync(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var book =  await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
                if (book == null)
                {
                    return NotFound();
                }
                Book = book;
               ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id");
                return Page();
            }

            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more information, see https://aka.ms/RazorPagesCRUD.
            public async Task<IActionResult> OnPostAsync()
            {
                var book = new BookDTO
                {
                    Name = Book.Title,
                    AuthorId = Book.AuthorId,
                    Image = Book.ImageBase64,
                    PublicDate = Book.PublishedDate
                };
                HttpClient client = new HttpClient();
                string api_endpoint = $"http://{HttpContext.Request.Host.ToString()}";

                var respone = await client.PutAsJsonAsync($"{api_endpoint}/api/Books/{Book.Id}",book);

                return RedirectToPage("./Index");
            }

            private bool BookExists(int id)
            {
                return _context.Books.Any(e => e.Id == id);
            }
        }
    }
