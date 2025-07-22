using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Authorize(Roles = "staff,admin")]
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
                    TotalCopies = b.BookCopies.Where(s => s.Status == "available").ToList().Count,
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
            var bookEntity = await _context.Books
                .Include(b => b.Categories)
                .Include(b => b.BookCopies) // Include bản sao để tính số lượng
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

            // Cập nhật Categories
            var newCategories = await _context.Categories
                .Where(c => book.CategoryIds.Contains(c.Id))
                .ToListAsync();

            bookEntity.Categories.Clear();
            foreach (var cat in newCategories)
            {
                bookEntity.Categories.Add(cat);
            }

            // ➕ Thêm số lượng bản sao mới nếu có
            if (book.NumberOfCopies > 0)
            {
                for (int i = 0; i < book.NumberOfCopies; i++)
                {
                    var newCopy = new BookCopy
                    {
                        BookId = bookEntity.Id,
                        Status = "available",
                        Condition = "new",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.BookCopies.Add(newCopy);
                }
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
                PublishedDate = dto.PublicDate,
                BookCopies = new List<BookCopy>()
            };

            // Gán categories
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                var categories = await _context.Categories
                    .Where(c => dto.CategoryIds.Contains(c.Id))
                    .ToListAsync();
                book.Categories = categories;
            }

            // Tạo các bản sao sách (BookCopy)
            for (int i = 0; i < dto.NumberOfCopies; i++)
            {
                book.BookCopies.Add(new BookCopy
                {
                    Status = "available",
                    Condition = "new",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
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
        [Authorize(Roles = "staff,admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.Include(s => s.BookCopies).FirstOrDefaultAsync(s => s.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            if(book.BookCopies.Count() > 0)
            {
                return BadRequest();
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
    [Required(ErrorMessage = "Tên sách không được để trống.")]
    public string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Tác giả không hợp lệ.")]
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Ảnh không được để trống.")]
    public string Image { get; set; }

    [Required(ErrorMessage = "Ngày phát hành không được để trống.")]
    public DateTime PublicDate { get; set; }

    [MinLength(1, ErrorMessage = "Phải chọn ít nhất một thể loại.")]
    public List<int> CategoryIds { get; set; } = new();

    [Range(1, 1000, ErrorMessage = "Số lượng bản sao phải từ 1 đến 1000.")]
    public int NumberOfCopies { get; set; }
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
