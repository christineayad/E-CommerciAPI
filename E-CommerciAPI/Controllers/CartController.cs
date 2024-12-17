using Azure.Core;
using E_CommerciAPI.Data;
using E_CommerciAPI.DTO;
using E_CommerciAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_CommerciAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost("[Action]")]
        public async Task<IActionResult> AddToCart(Cartdto cartdto)
        {
            // Get the current logged-in user
            // var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (cartdto.UserId == null)
            {
                return Unauthorized("User not logged in");
            }

            var user = await _userManager.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.CartProducts)
                .FirstOrDefaultAsync(u => u.Id == cartdto.UserId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var cart = await _context.Carts.Include(x => x.CartProducts).FirstOrDefaultAsync(c => c.UserId == cartdto.UserId);
          

            if (cart == null)
            {
              cart =  new Cart
                {
                    UserId = cartdto.UserId,
                    CartProducts = new List<CartProduct>()
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            //// Check if the product exists
            //var product = await _context.Products.FindAsync(productId);
            //if (product == null)
            //{
            //    return NotFound("Product not found");
            //}

            // Add or update the product in the cart
            var cartProduct = await _context.CartProducts
        .FirstOrDefaultAsync(cp => cp.CartId == cart.Id && cp.ProductId == cartdto.ProductId);
            if (cartProduct != null)
            {
                cartProduct.Quantity += cartdto.Quantity;
            }
            else
            {
                cartProduct = new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = cartdto.ProductId,
                    Quantity = cartdto.Quantity
                };
                await _context.CartProducts.AddAsync(cartProduct);
            }

            await _context.SaveChangesAsync();
            return Ok("Product added to cart");
           
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetCartProducts(string userId)
        {
            // Find the cart for the user and include products
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found for the user");
            }

            // Map the cart products to a DTO or return directly
            var cartProducts = cart.CartProducts.Select(cp => new
            {
                ProductId = cp.ProductId,
                ProductName = cp.Product.Name,
                ProductPrice = cp.Product.Price,
                Quantity = cp.Quantity,
                TotalPrice = cp.Product.Price * cp.Quantity
            });

            return Ok(cartProducts);
        }

        [HttpGet("[Action]")]
        //[Authorize]
        public async Task<IActionResult> GetMyCartTotalCost(string userId)
        {
        
            if (userId == null)
            {
                return Unauthorized("User not logged in");
            }

            // Fetch the cart
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found for the user");
            }

            // Calculate the total cost
            var totalCost = cart.CartProducts.Sum(cp => cp.Product.Price * cp.Quantity);

            return Ok(new { TotalCost = totalCost });
        }
        [HttpPost("[Action]")]
        public async Task<IActionResult> UpdateCartProduct(string userId, int productId)
        {
            
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(new { message = "cart not found" });
            }

          
            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);

            if (cartProduct == null)
            {
                return NotFound(new { message = "product not found in cart" });
            }

           
            if (cartProduct.Quantity > 1)
            {
                cartProduct.Quantity -= 1;  
            }
            else
            {
                
                _context.CartProducts.Remove(cartProduct);
            }

          
            await _context.SaveChangesAsync();

            return Ok(new { message = " Updated Cart Succeffuly" });
        }

        [HttpDelete("[Action]")]
        public async Task<IActionResult> RemoveAllCart(string userId)
        {
            
            var cart = await _context.Carts
                .Include(c => c.CartProducts)  
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(new { message = "cart not found." });
            }

           
            _context.CartProducts.RemoveRange(cart.CartProducts);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted Cart Succeffuly." });
        }

        [HttpPut("[Action]")]
        public async Task<IActionResult> UpdateCartProductQuantity(string userId, int productId, int quantity)
        {
          
            if (quantity <= 0)
            {
                return BadRequest(new { message = "Quantitity must be > 0" });
            }

           
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(new { message = "Cart NotFound" });
            }

            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);

            if (cartProduct == null)
            {
                return NotFound(new { message = "Product NotFound." });
            }

            cartProduct.Quantity = quantity;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Updated Cart Succeffuly" });
        }



    }
}

