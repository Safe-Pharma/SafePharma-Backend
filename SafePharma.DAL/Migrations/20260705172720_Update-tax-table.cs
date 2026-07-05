using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Updatetaxtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Taxes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "PharmacyId",
                table: "Taxes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_PharmacyId_Name",
                table: "Taxes",
                columns: new[] { "PharmacyId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Taxes_Pharmacies_PharmacyId",
                table: "Taxes",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taxes_Pharmacies_PharmacyId",
                table: "Taxes");

            migrationBuilder.DropIndex(
                name: "IX_Taxes_PharmacyId_Name",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "Taxes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
