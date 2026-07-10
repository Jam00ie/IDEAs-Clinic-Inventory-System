using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IdeasClinicInventory.Tests.Data;

public sealed class ApplicationDbContextModelTests
{
    private static ApplicationDbContext CreateContext()
    {
        // Building EF's model does not open this connection. A SQL Server provider is
        // specified so the test validates the same relational mapping used by the app.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(local);Database=ModelMetadataOnly;Trusted_Connection=True")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void Catalog_item_name_has_a_unique_index()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(CatalogItem));

        Assert.NotNull(entity);
        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual([nameof(CatalogItem.Name)]));
    }

    [Fact]
    public void Location_fields_have_a_composite_unique_index()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(InventoryLocation));
        var expectedProperties = new[]
        {
            nameof(InventoryLocation.RoomCode),
            nameof(InventoryLocation.StorageUnitLevel),
            nameof(InventoryLocation.StorageUnitCode),
            nameof(InventoryLocation.Subunit)
        };

        Assert.NotNull(entity);
        var locationIndex = Assert.Single(entity.GetIndexes(), index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(expectedProperties));
        Assert.Null(locationIndex.GetFilter());
    }

    [Theory]
    [InlineData(typeof(CatalogItem), nameof(CatalogItem.RowVersion))]
    [InlineData(typeof(InventoryLocation), nameof(InventoryLocation.RowVersion))]
    [InlineData(typeof(TrackedUnit), nameof(TrackedUnit.RowVersion))]
    [InlineData(typeof(UntrackedUnit), nameof(UntrackedUnit.RowVersion))]
    public void Row_version_is_a_database_generated_concurrency_token(
        Type entityType,
        string propertyName)
    {
        using var context = CreateContext();
        var property = context.Model.FindEntityType(entityType)?.FindProperty(propertyName);

        Assert.NotNull(property);
        Assert.True(property.IsConcurrencyToken);
        Assert.Equal(ValueGenerated.OnAddOrUpdate, property.ValueGenerated);
    }

    [Fact]
    public void Tracked_unit_identifier_is_unique_within_its_catalog_item()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(TrackedUnit));

        Assert.NotNull(entity);
        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(
                [nameof(TrackedUnit.CatalogItemId), nameof(TrackedUnit.Identifier)]));
    }

    [Fact]
    public void Tracked_unit_parents_use_restrict_delete_behavior()
    {
        using var context = CreateContext();
        var foreignKeys = context.Model.FindEntityType(typeof(TrackedUnit))?.GetForeignKeys();

        Assert.NotNull(foreignKeys);
        Assert.All(foreignKeys, foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }

    [Fact]
    public void Untracked_unit_is_unique_for_each_catalog_item_and_location()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(UntrackedUnit));

        Assert.NotNull(entity);
        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(
                [nameof(UntrackedUnit.CatalogItemId), nameof(UntrackedUnit.LocationId)]));
    }

    [Fact]
    public void Untracked_unit_parents_use_restrict_delete_behavior()
    {
        using var context = CreateContext();
        var foreignKeys = context.Model.FindEntityType(typeof(UntrackedUnit))?.GetForeignKeys();

        Assert.NotNull(foreignKeys);
        Assert.All(foreignKeys, foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }
}
