using Microsoft.EntityFrameworkCore;
using RealEstateSearchPortal.Models;

namespace RealEstateSearchPortal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favorite>()
            .HasKey(f => new { f.UserId, f.PropertyId });

        modelBuilder.Entity<Property>()
            .Property(p => p.Price)
            .HasPrecision(18, 2); 

        base.OnModelCreating(modelBuilder);
    }
}
