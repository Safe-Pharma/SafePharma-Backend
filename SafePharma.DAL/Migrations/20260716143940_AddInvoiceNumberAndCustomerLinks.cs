using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceNumberAndCustomerLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_CustomerId",
                table: "SaleItems",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Customers_CustomerId",
                table: "SaleItems",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Customers_CustomerId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_CustomerId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Sales");
        }
    }
}
