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
        public DateTime orderDate { get; set; } = DateTime.UtcNow;
       // public int userID { get; set; }
      //  public virtual AppUser? User { get; set; }
      
        public PaymentMethods Method { get; set; }
       // public ICollection<Product> Products { get; set; }
        public ICollection<OrderDetails> orderDetails { get; set; }
     
    }
}
