using Microsoft.EntityFrameworkCore;
using SmartHomeProject.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductAction> ProductActions { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemAction> ItemActions { get; set; }
    public DbSet<VoiceCommand> VoiceCommands { get; set; }
    public DbSet<Complaint> Complaints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasOne(i => i.Product)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.ProductId);

            entity.HasOne(i => i.User)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.UserId);

            entity.HasOne(i => i.Room)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RoomId);
        });

        modelBuilder.Entity<ItemAction>(entity =>
        {
            entity.HasOne(ia => ia.Item)
                .WithMany(i => i.Actions)
                .HasForeignKey(ia => ia.ItemId);
        });

        modelBuilder.Entity<ProductAction>(entity =>
        {
            entity.HasOne(pa => pa.Product)
                .WithMany(p => p.Actions)
                .HasForeignKey(pa => pa.ProductId);
        });
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