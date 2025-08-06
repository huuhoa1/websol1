using Microsoft.EntityFrameworkCore;
using webapp1.Models;

namespace webapp1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics", CreatedDate = new DateTime(2024, 12, 7) },
                new Product { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics", CreatedDate = new DateTime(2024, 12, 12) },
                new Product { Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics", CreatedDate = new DateTime(2024, 12, 17) },
                new Product { Id = 4, Name = "Monitor", Price = 299.99m, Category = "Electronics", CreatedDate = new DateTime(2024, 12, 22) },
                new Product { Id = 5, Name = "Desk Chair", Price = 149.99m, Category = "Furniture", CreatedDate = new DateTime(2024, 12, 27) }
            );
        }
    }
}