using System.ComponentModel.DataAnnotations;

namespace E_CommerciAPI.Model
{
    public class OrderDetails
    {
        
           
            public int Id { get; set; }
            public int ProductId { get; set; }
            public virtual Product? Product { get; set; }
            public int OrderId { get; set; }
            public virtual Order? order { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }

        
    }
}
