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
        [Required]
        public int CustomerId { get; set; }
        public required Customer? Customer { get; set; }
        [Required]
        public int ProductId { get; set; }
        
        public required Product? Product { get; set; }

    }
}
