using Microsoft.EntityFrameworkCore;

namespace ASP_EFC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
        public DbSet<Customer> Customers { get; set; }
    
    }
}
