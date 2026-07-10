using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeasClinicInventory.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackedUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackedUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogItemId = table.Column<int>(type: "int", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HomeLocationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedUnits", x => x.Id);
                    table.CheckConstraint("CK_TrackedUnits_Identifier_NotBlank", "LEN(LTRIM(RTRIM([Identifier]))) > 0");
                    table.ForeignKey(
                        name: "FK_TrackedUnits_CatalogItems_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "CatalogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackedUnits_InventoryLocations_HomeLocationId",
                        column: x => x.HomeLocationId,
                        principalTable: "InventoryLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedUnits_CatalogItemId_Identifier",
                table: "TrackedUnits",
                columns: new[] { "CatalogItemId", "Identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedUnits_HomeLocationId",
                table: "TrackedUnits",
                column: "HomeLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedUnits");
        }
    }
}
