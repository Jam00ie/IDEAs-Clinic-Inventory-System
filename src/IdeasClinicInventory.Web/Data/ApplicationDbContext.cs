using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Data;

/// <summary>
/// Represents the inventory application's SQL Server database session.
/// </summary>
/// <remarks>
/// Entity-specific SQL rules live in configuration classes, while this class
/// coordinates shared behavior such as timestamps and code normalization.
/// </remarks>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    public DbSet<InventoryLocation> InventoryLocations => Set<InventoryLocation>();

    public DbSet<TrackedUnit> TrackedUnits => Set<TrackedUnit>();

    public DbSet<UntrackedUnit> UntrackedUnits => Set<UntrackedUnit>();

    public DbSet<CatalogItemComponent> CatalogItemComponents => Set<CatalogItemComponent>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PrepareEntitiesForSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        PrepareEntitiesForSave();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration implementations from this project.
        // Adding a new entity configuration later requires no change to this method.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private void PrepareEntitiesForSave()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<CatalogItem>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.Name = entry.Entity.Name.Trim();
            }

            SetAuditTimestamps(entry, now);
        }

        foreach (var entry in ChangeTracker.Entries<InventoryLocation>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.NormalizeCodes();
            }

            SetAuditTimestamps(entry, now);
        }

        foreach (var entry in ChangeTracker.Entries<TrackedUnit>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.Identifier = entry.Entity.Identifier.Trim();
            }

            SetAuditTimestamps(entry, now);
        }

        foreach (var entry in ChangeTracker.Entries<UntrackedUnit>())
        {
            SetAuditTimestamps(entry, now);
        }

        foreach (var entry in ChangeTracker.Entries<CatalogItemComponent>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.Name = entry.Entity.Name.Trim();
            }

            SetAuditTimestamps(entry, now);
        }
    }

    private static void SetAuditTimestamps<TEntity>(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> entry,
        DateTimeOffset now)
        where TEntity : class
    {
        if (entry.State == EntityState.Added)
        {
            entry.Property(nameof(CatalogItem.CreatedAtUtc)).CurrentValue = now;
        }

        if (entry.State is EntityState.Added or EntityState.Modified)
        {
            entry.Property(nameof(CatalogItem.UpdatedAtUtc)).CurrentValue = now;
        }
    }
}
