using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResNet.Domain.Entities;

namespace Infrastructure.Data;

public class DataContext : IdentityDbContext<ApplicationUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ActionLog> ActionLogs { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<RestaurantRequest> RestaurantRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product - Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product - Restaurant
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Restaurant)
            .WithMany(r => r.Menu)
            .HasForeignKey(p => p.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItem - Product
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem - Order
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ActionLog - User
        modelBuilder.Entity<ActionLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.ActionLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table - Restaurant
        modelBuilder.Entity<Table>()
            .HasOne(t => t.Restaurant)
            .WithMany(r => r.Tables)
            .HasForeignKey(t => t.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Booking - Table
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Table)
            .WithMany()
            .HasForeignKey(b => b.TableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
