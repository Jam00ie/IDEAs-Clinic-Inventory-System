using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Tests.Models;

public sealed class UntrackedUnitTests
{
    [Fact]
    public void DisplayName_combines_catalog_item_and_location()
    {
        var unit = new UntrackedUnit
        {
            CatalogItem = new CatalogItem { Name = "Grove Kit" },
            Location = new InventoryLocation
            {
                RoomCode = "PSE2409A",
                StorageUnitLevel = 3,
                StorageUnitCode = "S2"
            }
        };

        Assert.Equal("Grove Kit Unit(s) @ PSE2409A-3S2", unit.DisplayName);
    }
}
