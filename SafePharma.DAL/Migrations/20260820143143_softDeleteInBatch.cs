using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class softDeleteInBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Pharmacies",
                newName: "IsActive");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Batches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Batches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Pharmacies",
                newName: "isActive");
        }
    }
}
