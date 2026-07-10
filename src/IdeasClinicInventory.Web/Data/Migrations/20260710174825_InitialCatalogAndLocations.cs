using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeasClinicInventory.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalogAndLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AllowedUseType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RestrictedToAdmins = table.Column<bool>(type: "bit", nullable: false),
                    QuantityReservedForAdmins = table.Column<int>(type: "int", nullable: false),
                    ReferenceImagePath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => x.Id);
                    table.CheckConstraint("CK_CatalogItems_QuantityReservedForAdmins_NonNegative", "[QuantityReservedForAdmins] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "InventoryLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StorageUnitCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StorageUnitLevel = table.Column<int>(type: "int", nullable: false),
                    Subunit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceImagePath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLocations", x => x.Id);
                    table.CheckConstraint("CK_InventoryLocations_StorageUnitLevel_NonNegative", "[StorageUnitLevel] >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_Name",
                table: "CatalogItems",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocations_RoomCode_StorageUnitLevel_StorageUnitCode_Subunit",
                table: "InventoryLocations",
                columns: new[] { "RoomCode", "StorageUnitLevel", "StorageUnitCode", "Subunit" },
                unique: true,
                filter: "[Subunit] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.DropTable(
                name: "InventoryLocations");
        }
    }
}
