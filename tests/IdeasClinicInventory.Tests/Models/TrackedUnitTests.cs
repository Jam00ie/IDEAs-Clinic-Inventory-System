using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Tests.Models;

public sealed class TrackedUnitTests
{
    [Fact]
    public void DisplayName_combines_catalog_name_and_identifier()
    {
        var unit = new TrackedUnit
        {
            CatalogItem = new CatalogItem { Name = "Arduino Kit" },
            Identifier = "A1"
        };

        Assert.Equal("Arduino Kit [A1]", unit.DisplayName);
    }
}
