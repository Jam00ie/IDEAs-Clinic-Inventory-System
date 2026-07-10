using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeasClinicInventory.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TightenCatalogAndLocationConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryLocations_RoomCode_StorageUnitLevel_StorageUnitCode_Subunit",
                table: "InventoryLocations");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocations_RoomCode_StorageUnitLevel_StorageUnitCode_Subunit",
                table: "InventoryLocations",
                columns: new[] { "RoomCode", "StorageUnitLevel", "StorageUnitCode", "Subunit" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogItems_Name_NotBlank",
                table: "CatalogItems",
                sql: "LEN(LTRIM(RTRIM([Name]))) > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryLocations_RoomCode_StorageUnitLevel_StorageUnitCode_Subunit",
                table: "InventoryLocations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogItems_Name_NotBlank",
                table: "CatalogItems");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocations_RoomCode_StorageUnitLevel_StorageUnitCode_Subunit",
                table: "InventoryLocations",
                columns: new[] { "RoomCode", "StorageUnitLevel", "StorageUnitCode", "Subunit" },
                unique: true,
                filter: "[Subunit] IS NOT NULL");
        }
    }
}
