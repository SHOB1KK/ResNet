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
    public DbSet<RestaurantCategory> RestaurantCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product - Category (многие к одному)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product - Restaurant (многие к одному)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Restaurant)
            .WithMany(r => r.Menu)
            .HasForeignKey(p => p.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restaurant - Category (многие ко многим через RestaurantCategory)
        modelBuilder.Entity<RestaurantCategory>()
            .HasKey(rc => new { rc.RestaurantId, rc.CategoryId });

        modelBuilder.Entity<RestaurantCategory>()
            .HasOne(rc => rc.Restaurant)
            .WithMany(r => r.RestaurantCategories)
            .HasForeignKey(rc => rc.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RestaurantCategory>()
            .HasOne(rc => rc.Category)
            .WithMany(c => c.RestaurantCategories)
            .HasForeignKey(rc => rc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItem - Product (многие к одному)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem - Order (многие к одному)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table - Restaurant (многие к одному)
        modelBuilder.Entity<Table>()
            .HasOne(t => t.Restaurant)
            .WithMany(r => r.Tables)
            .HasForeignKey(t => t.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Booking - Table (многие к одному)
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Table)
            .WithMany()
            .HasForeignKey(b => b.TableId)
            .OnDelete(DeleteBehavior.Cascade);

        // ActionLog - User (многие к одному)
        modelBuilder.Entity<ActionLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.ActionLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Table)
            .WithMany()
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkingHour>()
            .HasOne(wh => wh.RestaurantRequest)
            .WithMany(rr => rr.WorkingHours)
            .HasForeignKey(wh => wh.RestaurantRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Restaurant)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RestaurantId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}
