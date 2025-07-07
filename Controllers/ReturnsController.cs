using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnsController : ControllerBase
    {
        private readonly PrnContext _context;

        public ReturnsController(PrnContext context)
        {
            _context = context;
        }
        [HttpGet("rental-detail")]
        public async Task<ActionResult<DetailRentail>> Get(int rentailId)
        {
            var result = await _context.Returns
                .Include(rt=>rt.Rental)
                .ThenInclude(re=>re.Book)
                .ThenInclude(bc=>bc.Book)
                .FirstOrDefaultAsync(rt=>rt.RentalId==rentailId);
            if (result == null) return NotFound();
            var dto = new DetailRentail
            {
                RentalId = result.RentalId,
                ReturnId = result.Id,
                BookCopyId = result.Rental.BookCopyId,
                Title = result.Rental.Book.Book.Title,
                ImageBase64 = result.Rental.Book.Book.ImageBase64,
                RenewCount = result.Rental.RenewCount,
                RentailDate = result.Rental.RentalDate,
                DueDate = result.Rental.DueDate,
                BookCondition = result.Rental.Book.Condition,
                Status = result.Rental.Status
            };
            return dto;
        }
    }
}
