using System.ComponentModel.DataAnnotations;

namespace ASP_EFC.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [Phone]
        public required string PhoneNumber { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
