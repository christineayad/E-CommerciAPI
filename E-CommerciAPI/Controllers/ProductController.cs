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
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context) {
            _context = context;
        }
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products.Include(x => x.Category).Select(x=>new Productdto
            {
                Name = x.Name,
                Description= x.Description,
                Price= x.Price,
                CategoryId= x.CategoryId,


            }).ToListAsync();
            if (products == null) return NotFound();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        [HttpPost("[Action]")]
        public async Task<IActionResult> Create(Productdto pro)
        {
            if (pro == null)
                return NotFound();
            if(ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = pro.Name,
                    Description=pro.Description,
                    Price = pro.Price,
                    CategoryId=pro.CategoryId

                   
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Ok("Created Product Successfully");
            }
            return BadRequest(ModelState);
        }
      

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Productdto pro)
        {
           
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();
            existingProduct.Name = pro.Name;
            existingProduct.Description = pro.Description;
            existingProduct.Price = pro.Price;
            existingProduct.Category.Id = pro.CategoryId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> SearchProduct(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(new { message = "please enter Name...." });
            }

           
            var products = await _context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Category.Name.Contains(searchTerm))
                .ToListAsync();

            if (products.Count == 0)
            {
                return NotFound(new { message = "NotFound Product." });
            }

            return Ok(products);
        }


    }
}
