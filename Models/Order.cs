using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASP_EFC.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        
        public ICollection<Product> Products { get; set; }

    }
}
