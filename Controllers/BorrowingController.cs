using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
{

    [Route("api/borrowing")]
    [ApiController]
    public class BorrowingController : ControllerBase
    {
        private readonly PrnContext _context;

        public BorrowingController(PrnContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="user")]
        [HttpPost("rentals/request")]
        public IActionResult RequestRental(int bookId)
        {
            var user = HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            string username = user.Identity.Name;
            var user_obj =_context.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user_obj == null)
            {
                return Unauthorized();
            }

            var availableCopy = _context.BookCopies
                .FirstOrDefault(b => b.BookId == bookId && b.Status == "available");

            if (availableCopy == null)
                return BadRequest("Không còn bản sao nào khả dụng.");

            var rental = new Rental
            {
                UserId = user_obj.Id,
                BookCopyId = availableCopy.Id,
                RentalDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = "borrowed",
                RenewCount = 0
            };

            availableCopy.Status = "unavailable";

            _context.Rentals.Add(rental);
            _context.SaveChanges();

            return Ok("Đặt mượn sách thành công.");
        }

        [HttpGet("rentals/user/{userId}")]
        public IActionResult GetUserRentals(int userId)
        {
            var rentals = _context.Rentals
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .ThenInclude(bc => bc.Book)
                .Select(r => new
                {
                    BookTitle = r.Book.Book.Title,
                    RentalDate = r.RentalDate,
                    DueDate = r.DueDate,
                    Status = r.Status,
                    RenewCount = r.RenewCount
                })
                .ToList();

            return Ok(rentals);
        }

        [HttpGet("history/user/{userId}")]
        public IActionResult GetUserBorrowingHistory(int userId)
        {
            var history = _context.Rentals
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .ThenInclude(bc => bc.Book)
                .Select(r => new
                {
                    BookTitle = r.Book.Book.Title,
                    RentalDate = r.RentalDate,
                    DueDate = r.DueDate,
                    Status = r.Status,
                    RenewCount = r.RenewCount
                })
                .OrderByDescending(r => r.RentalDate)
                .ToList();

            return Ok(history);
        }

        [HttpGet("bookdetails/{bookId}")]
        public IActionResult GetBookDetail(int bookId)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCopies)
                .AsNoTracking()
                .FirstOrDefault(b => b.Id == bookId);

            if (book == null) return NotFound();

            var bookInfo = new
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author.Name,
                Year = book.PublishedDate.Year,
                Description = "Sách thư viện - " + book.Title,
                ImageUrl = book.ImageBase64,
                LibraryName = "Thư Viện của thầy Thọ",
                TotalCopies = book.BookCopies.Count,
                AvailableCopies = book.BookCopies.Count(c => c.Status == "available"),
                BorrowedCopies = book.BookCopies.Count(c => c.Status == "unavailable"),
                QueueCount = _context.Rentals.Count(r => r.Book.BookId == bookId && r.Status == "borrowed"),
                NextAvailableDate = _context.Rentals
                    .Where(r => r.Book.BookId == bookId && r.Status == "borrowed")
                    .OrderBy(r => r.DueDate)
                    .Select(r => r.DueDate)
                    .FirstOrDefault()
            };

            return Ok(bookInfo);
        }
    }
}
