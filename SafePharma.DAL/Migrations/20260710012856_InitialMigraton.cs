using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigraton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Medicines_MedicineId",
                table: "Batches");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "Batches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseReceiptItemId",
                table: "Batches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Batches_PurchaseReceiptItemId",
                table: "Batches",
                column: "PurchaseReceiptItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_PharmacyMedicines_MedicineId",
                table: "Batches",
                column: "MedicineId",
                principalTable: "PharmacyMedicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_PurchaseReceiptItems_PurchaseReceiptItemId",
                table: "Batches",
                column: "PurchaseReceiptItemId",
                principalTable: "PurchaseReceiptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_PharmacyMedicines_MedicineId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Batches_PurchaseReceiptItems_PurchaseReceiptItemId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_PurchaseReceiptItemId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PurchaseReceiptItemId",
                table: "Batches");

            migrationBuilder.AlterColumn<int>(
                name: "BatchNumber",
                table: "Batches",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Medicines_MedicineId",
                table: "Batches",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
