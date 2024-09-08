using System.ComponentModel.DataAnnotations;

namespace ASP_EFC.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public decimal Price { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
