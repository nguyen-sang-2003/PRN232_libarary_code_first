using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        //[Authorize("Role")]
        [Authorize(Roles = "staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewDTO>>> GetBooks()
        {
           
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCopies)
                .Include(b => b.Categories)
                .Select(b => new BookViewDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishedDate = b.PublishedDate,
                    ImageBase64 = b.ImageBase64,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author.Name,
                    TotalCopies = b.BookCopies.Count,
                    categories = b.Categories.Select(c => new Category
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList()

                })
                .ToListAsync();
          
           
            return books;
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
            // Tìm entity sách + load luôn các Category
            var bookEntity = await _context.Books
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bookEntity == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            bookEntity.Title = book.Name;
            bookEntity.AuthorId = book.AuthorId;
            bookEntity.ImageBase64 = book.Image;
            bookEntity.PublishedDate = book.PublicDate;

            // Cập nhật quan hệ Category (many-to-many)
            var newCategories = await _context.Categories
                .Where(c => book.CategoryIds.Contains(c.Id))
                .ToListAsync();

            // Cập nhật đúng cách: xóa hết cũ rồi add lại
            bookEntity.Categories.Clear();         // Xóa quan hệ cũ
            foreach (var cat in newCategories)     // Thêm mới
            {
                bookEntity.Categories.Add(cat);
            }

            try
            {
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> CreateBook([FromBody] BookDTO dto)
        {
            var book = new Book
            {
                Title = dto.Name,
                AuthorId = dto.AuthorId,
                ImageBase64 = dto.Image,
                PublishedDate = dto.PublicDate
            };

            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                var categories = await _context.Categories
                    .Where(c => dto.CategoryIds.Contains(c.Id))
                    .ToListAsync();
                book.Categories = categories;
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = new BookResponseDTO
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ImageBase64 = book.ImageBase64,
                PublishedDate = book.PublishedDate,
                CategoryIds = book.Categories?.Select(c => c.Id).ToList()
            };

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, result);
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
    public string Image { get; set; }
    public DateTime PublicDate { get; set; }
    public List<int> CategoryIds { get; set; } = new();

}
public class BookViewDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime PublishedDate { get; set; }
    public string ImageBase64 { get; set; }

    public int AuthorId { get; set; }
    public string AuthorName { get; set; }

    public int TotalCopies { get; set; }
    public List<Category> categories { get; set; } = new List<Category>();
}
public class BookResponseDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageBase64 { get; set; }
    public DateTime PublishedDate { get; set; }
    public int AuthorId { get; set; }
    public List<int> CategoryIds { get; set; }
}
