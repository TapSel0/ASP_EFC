using Microsoft.EntityFrameworkCore;

namespace ASP_EFC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // Указываем, что связь необязательна

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Orders)
                .WithOne(o => o.Product);

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Customer>().HasData(
            //    new Customer { Id = 2, Email = "joe.boo@example.com", Name = "Joe Boodoosh", PhoneNumber = "154135135234" }
            //    );

            //modelBuilder.Entity<Product>().HasData(
            //    new Product { Id = 1, Name = "Potato", Price = 12.22m },
            //    new Product { Id = 2, Name = "Headphones", Price = 43.00m }
            //    );
        }


    }
}
