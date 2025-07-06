using LibararyWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
{
    [Route("/api")]
    [ApiController]
    public class BorrowingController : ControllerBase
    {
        private readonly PrnContext _context;
        public BorrowingController(PrnContext context)
        {
            _context = context;
        }
        [HttpPost("rentals/request")]
        public IActionResult RequestRental(int userId, int bookId)
        {
            var availableCopy = _context.BookCopies
                .FirstOrDefault(b => b.BookId == bookId && b.Status == "available");

            if (availableCopy == null)
                return BadRequest("Không còn bản sao nào khả dụng.");

            var rental = new Rental
            {
                UserId = userId,
                BookCopyId = availableCopy.Id,
                RentalDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = "borrowed"
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
                .Where(r => r.UserId == userId && r.Status == "borrowed")
                .Include(r => r.Book)              // BookCopy
                .ThenInclude(bc => bc.Book)        // Book
                .Select(r => new
                {
                    BookTitle = r.Book.Book.Title, // BookCopy.Book.Title
                    Condition = r.Book.Condition,
                    RentalDate = r.RentalDate,
                    DueDate = r.DueDate,
                    Status = r.Status
                })
                .ToList();

            return Ok(rentals);
        }

    }
}
