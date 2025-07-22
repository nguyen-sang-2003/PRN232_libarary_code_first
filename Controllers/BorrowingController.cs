using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static PrnContext;

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
                Status = RentalStatus.Pending,
                RenewCount = 0
            };

            availableCopy.Status = BookCopyStatus.Unavailable;

            _context.Rentals.Add(rental);
            _context.SaveChanges();

            return Ok(new {
                message = "Borrowing success.",
                userId = user_obj.Id,
                username = user_obj.Username,
                bookId = bookId
            });
        }

        [Authorize(Roles = "user")]
        [HttpPost("rentals/cancel/{rentalId}")]
        public IActionResult CancelRental(int rentalId)
        {
            var user = HttpContext.User;
            string username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Username not found in token");

            var user_obj = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user_obj == null)
                return Unauthorized($"User '{username}' not found in database");

            var rental = _context.Rentals
                .Include(r => r.Book)
                .FirstOrDefault(r => r.Id == rentalId && r.UserId == user_obj.Id);

            if (rental == null)
                return NotFound("Rental not found.");

            if (rental.Status != RentalStatus.Pending)
                return BadRequest("Only pending rentals can be cancelled.");

            // Trả lại trạng thái available cho BookCopy
            var bookCopy = _context.BookCopies.FirstOrDefault(bc => bc.Id == rental.BookCopyId);
            if (bookCopy != null)
                bookCopy.Status = BookCopyStatus.Available;

            // Đổi status thành Cancelled
            rental.Status = RentalStatus.Cancelled;
            rental.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Ok(new { message = "Rental cancelled and book copy is now available." });
        }

        [Authorize(Roles = "user")]
        [HttpPost("rentals/renew/{rentalId}")]
        public IActionResult RenewRental(int rentalId)
        {
            var user = HttpContext.User;
            string username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Username not found in token");

            var user_obj = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user_obj == null)
                return Unauthorized($"User '{username}' not found in database");

            var rental = _context.Rentals.FirstOrDefault(r => r.Id == rentalId && r.UserId == user_obj.Id);
            if (rental == null)
                return NotFound("Rental not found.");

            if (rental.Status != RentalStatus.Approved)
                return BadRequest("Only approved rentals can be renewed.");

            // Optional: Check renew limit
            int maxRenew = 4;
            if (rental.RenewCount >= maxRenew)
                return BadRequest("Renewal limit reached.");

            // Extend due date (e.g., +7 days)
            rental.DueDate = rental.DueDate.AddDays(7);
            rental.RenewCount += 1;
            rental.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Ok(new { message = "Rental renewed successfully!", newDueDate = rental.DueDate });
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
                    Id = r.Id, // Bổ sung trường Id
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
