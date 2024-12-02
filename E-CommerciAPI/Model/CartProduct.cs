using System.ComponentModel.DataAnnotations;

namespace E_CommerciAPI.Model
{
    public class CartProduct
    {
        [Key]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
