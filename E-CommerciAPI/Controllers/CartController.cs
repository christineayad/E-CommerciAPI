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
            //var cart = await _context.Carts.Include(c=>c.CartProducts).SingleOrDefaultAsync(c=>c.UserId==pro.UserId);
            //if (cart == null)
            //{
            //    cart = new Cart
            //    {
            //        UserId = pro.UserId,
            //        CartProducts = new List<CartProduct>()
            //    };
            //    _context.Carts.AddAsync(cart);
            //    await _context.SaveChangesAsync();
            //}
            //var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == pro.ProductId);

            //if (cartProduct == null)
            //{
            //    cartProduct = new CartProduct
            //    {
            //        CartId = cart.Id,
            //        ProductId = pro.ProductId,
            //        Quantity = 1
            //    };
            //    _context.CartProducts.Add(cartProduct);
            //}
            //else
            //{
            //    cartProduct.Quantity ++;
            //}

            //await _context.SaveChangesAsync();

            //return Ok(new { message = "Product added to cart successfully." });
        }

        [HttpGet("{userId}/products")]
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

        [HttpGet("TotalCost")]
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


    }
}

