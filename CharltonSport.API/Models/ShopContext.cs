using Microsoft.EntityFrameworkCore;

namespace CharltonSport.API.Models
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);
            modelBuilder.Seed();
        }
        public DbSet<Product> Products { get; set; } 
        public DbSet<Category> Categories { get; set; }
    }

    
}
