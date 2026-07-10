using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class edited_phi_pri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrdersItems_Medicines_MedicineId",
                table: "PurchaseOrdersItems");

            migrationBuilder.RenameColumn(
                name: "MedicineId",
                table: "PurchaseReceiptItems",
                newName: "PharmacyMedicineId");

            migrationBuilder.RenameColumn(
                name: "MedicineId",
                table: "PurchaseOrdersItems",
                newName: "PharmacyMedicineId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrdersItems_MedicineId",
                table: "PurchaseOrdersItems",
                newName: "IX_PurchaseOrdersItems_PharmacyMedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItems_PharmacyMedicineId",
                table: "PurchaseReceiptItems",
                column: "PharmacyMedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrdersItems_PharmacyMedicines_PharmacyMedicineId",
                table: "PurchaseOrdersItems",
                column: "PharmacyMedicineId",
                principalTable: "PharmacyMedicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseReceiptItems_PharmacyMedicines_PharmacyMedicineId",
                table: "PurchaseReceiptItems",
                column: "PharmacyMedicineId",
                principalTable: "PharmacyMedicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrdersItems_PharmacyMedicines_PharmacyMedicineId",
                table: "PurchaseOrdersItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseReceiptItems_PharmacyMedicines_PharmacyMedicineId",
                table: "PurchaseReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReceiptItems_PharmacyMedicineId",
                table: "PurchaseReceiptItems");

            migrationBuilder.RenameColumn(
                name: "PharmacyMedicineId",
                table: "PurchaseReceiptItems",
                newName: "MedicineId");

            migrationBuilder.RenameColumn(
                name: "PharmacyMedicineId",
                table: "PurchaseOrdersItems",
                newName: "MedicineId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrdersItems_PharmacyMedicineId",
                table: "PurchaseOrdersItems",
                newName: "IX_PurchaseOrdersItems_MedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrdersItems_Medicines_MedicineId",
                table: "PurchaseOrdersItems",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
