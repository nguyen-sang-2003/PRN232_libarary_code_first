using LibararyWebApplication.DTO.Cuong;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers.Cuong
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly PrnContext _context;

        public CategoriesController(PrnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }

        // [HttpGet("{id}/books")]
        // public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(int id)
        // {
        //     var category = await _context.Categories
        //         .Include(c => c.BookCategories)
        //         .ThenInclude(bc => bc.Book)
        //             .ThenInclude(b => b.Author)
        //         .Include(c => c.BookCategories)
        //         .ThenInclude(bc => bc.Book)
        //             .ThenInclude(b => b.BookCopies)
        //         .FirstOrDefaultAsync(c => c.Id == id);

        //     if (category == null)
        //     {
        //         return NotFound();
        //     }

        //     var books = category.BookCategories
        //         .Select(bc => bc.Book)
        //         .Select(b => new BookDto
        //         {
        //             Id = b.Id,
        //             Title = b.Title,
        //             AuthorName = b.Author.Name,
        //             ImageBase64 = b.ImageBase64,
        //             PublishedDate = b.PublishedDate,
        //             TotalCopies = b.BookCopies.Count,
        //             AvailableCopies = b.BookCopies.Count(c => c.Status == "available")
        //         })
        //         .ToList();

        //     return Ok(books);
        // }
    }

}
