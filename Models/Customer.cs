using System.ComponentModel.DataAnnotations;

namespace ASP_EFC.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
