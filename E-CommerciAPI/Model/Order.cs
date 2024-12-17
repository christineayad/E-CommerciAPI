namespace E_CommerciAPI.Model
{
    public enum PaymentMethods
    {
        chash,
        creditCard
    }
    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime orderDate { get; set; }
        public string UserId { get; set; }
        public AppUser? AppUser { get; set; }

        public PaymentMethods Method { get; set; }
     public ICollection<Product> Products { get; set; }
     public ICollection<OrderDetails> orderDetails { get; set; }

       
    }
}
