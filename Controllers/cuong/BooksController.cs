using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibararyWebApplication.DTO.Cuong;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers.Cuong
{
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly PrnContext _context;

        public BooksController(PrnContext context)
        {
            _context = context;
        }


        [HttpGet("/api/cuong/books/{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCopies)
                .Where(b => b.Id == id)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.Name,
                    ImageBase64 = b.ImageBase64,
                    PublishedDate = b.PublishedDate,
                    TotalCopies = b.BookCopies.Count,
                    AvailableCopies = b.BookCopies.Count(c => c.Status == "available")
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        //Day du lieu Book len Home Page
        [HttpGet("/api/cuong/books")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksInHomePage([FromQuery] string? search)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCopies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(lowerSearch)
                );
            }

            var books = await query
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.Name,
                    ImageBase64 = b.ImageBase64,
                    PublishedDate = b.PublishedDate,
                    TotalCopies = b.BookCopies.Count,
                    AvailableCopies = b.BookCopies.Count(c => c.Status == "available")
                })
                .ToListAsync();

            return Ok(books);
        }
    }
}
