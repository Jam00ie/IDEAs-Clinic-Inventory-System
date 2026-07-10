using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeasClinicInventory.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUntrackedUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UntrackedUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogItemId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    QuantityAtLocation = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UntrackedUnits", x => x.Id);
                    table.CheckConstraint("CK_UntrackedUnits_QuantityAtLocation_NonNegative", "[QuantityAtLocation] >= 0");
                    table.ForeignKey(
                        name: "FK_UntrackedUnits_CatalogItems_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "CatalogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UntrackedUnits_InventoryLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "InventoryLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UntrackedUnits_CatalogItemId_LocationId",
                table: "UntrackedUnits",
                columns: new[] { "CatalogItemId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UntrackedUnits_LocationId",
                table: "UntrackedUnits",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UntrackedUnits");
        }
    }
}
