using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRentalsController : ControllerBase
    {
        private readonly PrnContext _context;

        public UserRentalsController(PrnContext context)
        {
            _context = context;
        }
        [HttpPost("/api/users/rental/renew-book")]
        public async Task<ActionResult> UpdateRenewBook(int rentalId)
        {
            var result = await _context.Rentals.FirstOrDefaultAsync(rt => rt.Id == rentalId);
            if (result == null) return NotFound();

            result.RenewCount = result.RenewCount == null ? 1 : result.RenewCount + 1;

            _context.Entry(result).Property(u => u.RenewCount).IsModified = true;
            await _context.SaveChangesAsync();

            return Ok(result);
        }
    }
}
