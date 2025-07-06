using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly PrnContext _context;

        public BooksController(PrnContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.Include(s => s.Author).Include(s => s.BookCopies).ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookDTO book)
        {
            // Tìm entity cũ trong DB
            var bookEntity = await _context.Books.FindAsync(id);
            if (bookEntity == null)
            {
                return NotFound();
            }

            // Cập nhật các trường
            bookEntity.Title = book.Name;
            bookEntity.AuthorId = book.AuthorId;
            bookEntity.ImageBase64 = book.Image;
            bookEntity.PublishedDate = book.PublicDate;

            try
            {
                await _context.SaveChangesAsync(); // Entity đã được theo dõi -> EF tự nhận biết thay đổi
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookDTO book)
        {
            var book1 = new Book
            {
               Title = book.Name,
               AuthorId = book.AuthorId,
               ImageBase64 = book.Image,
               PublishedDate = book.PublicDate
            };

            _context.Books.Add(book1);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book1.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
public class BookDTO
{
    public string Name { get; set; }
    public int AuthorId { get; set; }
    public string Image {  get; set; }
    public DateTime PublicDate { get; set; }
}
