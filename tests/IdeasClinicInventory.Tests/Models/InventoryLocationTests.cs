using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Tests.Models;

public sealed class InventoryLocationTests
{
    [Theory]
    [InlineData("0", 0, "0", null, "0-00")]
    [InlineData("E52112", 0, "0", null, "E52112-00")]
    [InlineData("E52112", 1, "0", null, "E52112-00")]
    [InlineData("PSE2409A", 3, "S2", null, "PSE2409A-3S2")]
    [InlineData(" pse2409a ", 3, " s2 ", " b4 ", "PSE2409A-3S2-B4")]
    public void LocationCode_is_built_from_structured_fields(
        string roomCode,
        int level,
        string storageUnitCode,
        string? subunit,
        string expected)
    {
        var location = new InventoryLocation
        {
            RoomCode = roomCode,
            StorageUnitLevel = level,
            StorageUnitCode = storageUnitCode,
            Subunit = subunit
        };

        Assert.Equal(expected, location.LocationCode);
    }
}
