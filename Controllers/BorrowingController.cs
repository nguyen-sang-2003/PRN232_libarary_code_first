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
                return Unauthorized("User not authenticated");
            }

            string username = user.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Username not found in token");
            }

            var user_obj = _context.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user_obj == null)
            {
                return Unauthorized($"User '{username}' not found in database");
            }

            var availableCopy = _context.BookCopies
                .FirstOrDefault(b => b.BookId == bookId && b.Status == "available");

            if (availableCopy == null)
                return BadRequest("No copy available.");

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

            return Ok(new {
                message = "Borrowing success.",
                userId = user_obj.Id,
                username = user_obj.Username,
                bookId = bookId
            });
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

        [Authorize(Roles = "user")]
        [HttpGet("history/current")]
        public IActionResult GetCurrentUserBorrowingHistory()
        {
            var user = HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            string username = user.Identity.Name;
            var user_obj = _context.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user_obj == null)
            {
                return Unauthorized();
            }

            var history = _context.Rentals
                .Where(r => r.UserId == user_obj.Id)
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


    }
}
