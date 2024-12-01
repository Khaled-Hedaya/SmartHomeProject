using Microsoft.EntityFrameworkCore;
using SmartHomeProject.Models;

namespace SmartHomeProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemAction> ItemActions { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public DbSet<Product> Products { get; set; }
public DbSet<ProductAction> ProductActions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(i => i.User)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Room)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RoomId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ItemAction>()
                .HasOne(a => a.Item)
                .WithMany(i => i.Actions)
                .HasForeignKey(a => a.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ProductAction>()
    .HasOne(pa => pa.Product)
    .WithMany(p => p.Actions)
    .HasForeignKey(pa => pa.ProductId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}