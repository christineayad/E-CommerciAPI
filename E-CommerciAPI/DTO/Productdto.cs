using E_CommerciAPI.Model;
using System.ComponentModel.DataAnnotations;

namespace E_CommerciAPI.DTO
{
    public class Productdto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public int CategoryId { get; set; }
       
    }
}
