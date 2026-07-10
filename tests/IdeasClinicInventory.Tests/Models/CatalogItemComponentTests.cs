using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Tests.Models;

public sealed class CatalogItemComponentTests
{
    [Fact]
    public void DisplayName_contains_name_and_expected_quantity()
    {
        var component = new CatalogItemComponent
        {
            Name = "Jumper Wire",
            ExpectedQuantity = 10
        };

        Assert.Equal("Jumper Wire (10)", component.DisplayName);
    }

    [Fact]
    public void Catalog_item_with_no_components_represents_a_whole_item()
    {
        var item = new CatalogItem();

        Assert.Empty(item.Components);
    }
}
