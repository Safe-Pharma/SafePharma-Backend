using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatePharmacyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Pharmacies_PharmacyId",
                table: "Batches");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Pharmacies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyId",
                table: "Batches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Pharmacies_PharmacyId",
                table: "Batches",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Pharmacies_PharmacyId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Pharmacies");

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyId",
                table: "Batches",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Pharmacies_PharmacyId",
                table: "Batches",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id");
        }
    }
}
