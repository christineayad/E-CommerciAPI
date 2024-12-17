using E_CommerciAPI.Data;
using E_CommerciAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace E_CommerciAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession(string userId)
        {
         
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID is required." });
            }
          

          
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

       
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartProducts.Any())
            {
                return NotFound(new { message = "No items in the cart for this user." });
            }

           
            var totalAmount = cart.CartProducts
                .Sum(item => item.Product.Price * item.Quantity);

            try
            {
                // Create a PaymentIntent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(totalAmount * 100), // Amount in cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
            {
                { "UserId", userId },
                { "CustomerEmail", user.Email }
            }
                };

                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = await service.CreateAsync(options);

                // Create the order
                var order = new Order
                {
                    Amount = (long)(totalAmount),
                    orderDate = DateTime.Now,
                    Method = PaymentMethods.creditCard,
                   UserId = userId
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create OrderDetails for each cart product
                foreach (var item in cart.CartProducts)
                {
                    var detail = new OrderDetails
                    {
                        OrderId = order.Id,
                        Price = item.Product.Price,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    _context.OrderDetails.Add(detail);
                }

               
                await _context.SaveChangesAsync();

                // Clear the cart
                _context.CartProducts.RemoveRange(cart.CartProducts);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                // Return the client secret for the payment intent
                return Ok(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, new { message = "An error occurred while creating the payment session.", error = ex.Message });
            }
        }


       
       
        [HttpGet("list-payments")]
        public async Task<IActionResult> ListPayments()
        {
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentListOptions
                {
                    Limit = 10,  // Adjust this limit as necessary
                };

                var paymentIntents = await service.ListAsync(options);

                // Check if there are any payment intents
                if (paymentIntents.Data.Count == 0)
                {
                    return Ok(new { message = "No payments found." });
                }

                return Ok(paymentIntents);
            }
            catch (StripeException e)
            {
                return StatusCode(500, new { error = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }
    }
}

