using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibararyWebApplication.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly PrnContext _context;

        public RentalsController(PrnContext context)
        {
            _context = context;
        }

        // GET: api/Rentals
        [HttpGet]
        [Authorize(Roles = "staff")]
        public async Task<ActionResult<List<RentalDTO>>> GetRentals()
        {
            var rental = await _context.Rentals.Include(s => s.User).Include(s => s.Book).Select(s => new RentalDTO
            {
                Id = s.Id,
                Status = s.Status,
                RentalDate = s.RentalDate,
                DueDate = s.DueDate,
                CreatedAt = s.CreatedAt,
                BookCopyId = s.BookCopyId,
                UpdatedAt = s.UpdatedAt,
                UserName = s.User.Username,
               BookName = s.Book.Book.Title,
            }).ToListAsync();
            return Ok(rental);
        }

        // GET: api/Rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rentals.Include(s => s.User).Include(s => s.Book).FirstOrDefaultAsync(s => s.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return rental;
        }

        // PUT: api/Rentals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRental(int id, String Status,[FromBody]int CopyId)
        {
            var rental = _context.Rentals.Include(s => s.Book).FirstOrDefault(s => s.Id == id);
            rental.Status = Status;
             rental.Book.Status = "available";

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
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

        // POST: api/Rentals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rental>> PostRental(Rental rental)
        {
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRental", new { id = rental.Id }, rental);
        }

        // DELETE: api/Rentals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}
