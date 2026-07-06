using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedingMedicines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradeNameAr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TradeNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitOfSale = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitsPerPackage = table.Column<int>(type: "int", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TaxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false),
                    IsPrescriptionRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsControlled = table.Column<bool>(type: "bit", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StorageConditions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TherapeuticCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                    table.CheckConstraint("CK_Medicine_PurchasePrice", "[PurchasePrice] >= 0");
                    table.CheckConstraint("CK_Medicine_SellingPrice", "[SellingPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_Medicines_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ScientificName",
                table: "Medicines",
                column: "ScientificName");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_TaxId",
                table: "Medicines",
                column: "TaxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicines");
        }
    }
}
