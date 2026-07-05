using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Update_PharmacySettings_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacySettings_Pharmacies_PharmacyId",
                table: "PharmacySettings");

            migrationBuilder.DropIndex(
                name: "IX_PharmacySettings_PharmacyId",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "City",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "PharmacySettings");

            migrationBuilder.DropColumn(
                name: "TaxRegistrationNumber",
                table: "PharmacySettings");

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyId",
                table: "PharmacySettings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacySettings_PharmacyId",
                table: "PharmacySettings",
                column: "PharmacyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacySettings_Pharmacies_PharmacyId",
                table: "PharmacySettings",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacySettings_Pharmacies_PharmacyId",
                table: "PharmacySettings");

            migrationBuilder.DropIndex(
                name: "IX_PharmacySettings_PharmacyId",
                table: "PharmacySettings");

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyId",
                table: "PharmacySettings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxRegistrationNumber",
                table: "PharmacySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacySettings_PharmacyId",
                table: "PharmacySettings",
                column: "PharmacyId",
                unique: true,
                filter: "[PharmacyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacySettings_Pharmacies_PharmacyId",
                table: "PharmacySettings",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id");
        }
    }
}
