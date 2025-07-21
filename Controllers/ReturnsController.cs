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
            //var result = await _context.Returns
            //    .Include(rt=>rt.Rental)
            //    .ThenInclude(re=>re.Book)
            //    .ThenInclude(bc=>bc.Book)
            //    .FirstOrDefaultAsync(rt=>rt.RentalId==rentailId);
            //if (result == null) return NotFound();
            //var dto = new DetailRentail
            //{
            //    RentalId = result.RentalId,
            //    Title = result.Rental.Book.Book.Title,
            //    ImageBase64 = result.Rental.Book.Book.ImageBase64,
            //    RenewCount = result.Rental.RenewCount,
            //    RentailDate = result.Rental.RentalDate,
            //    DueDate = result.Rental.DueDate,
            //    BookCondition = result.Rental.Book.Condition,
            //    Status = result.Rental.Status
            //};
            var re = await _context.Rentals.Include(r=>r.Book).ThenInclude(bc => bc.Book).FirstOrDefaultAsync(r => r.Id == rentailId);
            if(re == null) return NotFound();
            var DTO = new DetailRentail
            {
                RentalId = re.Id,
                Title = re.Book.Book.Title,
                ImageBase64 = re.Book.Book.ImageBase64,
                RenewCount = re.RenewCount,
                RentailDate = re.RentalDate,
                DueDate = re.DueDate,
                BookCondition = re.Book.Condition,
                Status = re.Status
            };
            return DTO;
        }
    }
}
