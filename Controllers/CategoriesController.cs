using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibararyWebApplication.Controllers
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

        // GET: api/Categories
        //[Authorize(Roles = "staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<categoryDTO>>> GetCategories()
        {
            var cate = await _context.Categories.Include(s => s.Books).ToListAsync();
            var categories = cate.Select(c => new categoryDTO
            {
                id = c.Id,
                Name = c.Name
            }).ToList();
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, string Name)
        {
            var category = _context.Categories.FirstOrDefault(s => s.Id == id);
            category.Name = Name;
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "staff,admin")]
        public async Task<ActionResult<Category>> PostCategory([FromBody]categoryDTO category)
        {
            Category category1 = new Category
            {
                Name = category.Name
            };
            _context.Categories.Add(category1);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category1.Id }, category);
        }

        // DELETE: api/Categories/5
        [Authorize(Roles = "staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

public class categoryDTO
{
    public int id  { get; set; }
    public string Name { get; set; }

}
