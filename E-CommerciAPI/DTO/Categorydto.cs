using System.ComponentModel.DataAnnotations;

namespace E_CommerciAPI.DTO
{
    public class Categorydto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
