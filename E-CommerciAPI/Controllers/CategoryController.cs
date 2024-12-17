using E_CommerciAPI.Data;
using E_CommerciAPI.DTO;
using E_CommerciAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerciAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context) {
            _context = context;
        }
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        [HttpPost("[Action]")]
        public async Task<IActionResult> Create(Categorydto cat)
        {
            if (cat == null)
                return NotFound();
            if(ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = cat.Name,
                   
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Ok("Created Category Successfully");
            }
            return BadRequest(ModelState);
        }
      

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Categorydto category)
        {
           
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                return NotFound();
            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> SearchCategory(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(new { message = "please enter Name...." });
            }


            var categories = await _context.Categories
                .Where(p => p.Name.Contains(searchTerm))
                .ToListAsync();

            if (categories.Count == 0)
            {
                return NotFound(new { message = "NotFound Categories." });
            }

            return Ok(categories);
        }

    }
}
