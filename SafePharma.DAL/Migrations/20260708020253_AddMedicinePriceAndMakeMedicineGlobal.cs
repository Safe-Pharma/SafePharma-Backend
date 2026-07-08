using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicinePriceAndMakeMedicineGlobal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Taxes_TaxId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_TaxId",
                table: "Medicines");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Medicine_PurchasePrice",
                table: "Medicines");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Medicine_SellingPrice",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Medicines");

            migrationBuilder.CreateTable(
                name: "MedicinePrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinePrices", x => x.Id);
                    table.CheckConstraint("CK_MedicinePrice_PurchasePrice", "[PurchasePrice] >= 0");
                    table.CheckConstraint("CK_MedicinePrice_SellingPrice", "[SellingPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_MedicinePrices_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicinePrices_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicinePrices_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_TradeNameEn",
                table: "Medicines",
                column: "TradeNameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePrices_MedicineId_PharmacyId",
                table: "MedicinePrices",
                columns: new[] { "MedicineId", "PharmacyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePrices_PharmacyId",
                table: "MedicinePrices",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePrices_TaxId",
                table: "MedicinePrices",
                column: "TaxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicinePrices");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_TradeNameEn",
                table: "Medicines");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Medicines",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellingPrice",
                table: "Medicines",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "TaxId",
                table: "Medicines",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_TaxId",
                table: "Medicines",
                column: "TaxId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Medicine_PurchasePrice",
                table: "Medicines",
                sql: "[PurchasePrice] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Medicine_SellingPrice",
                table: "Medicines",
                sql: "[SellingPrice] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Taxes_TaxId",
                table: "Medicines",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
