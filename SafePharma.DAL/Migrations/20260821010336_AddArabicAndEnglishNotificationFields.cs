using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddArabicAndEnglishNotificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Pharmacies",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notifications",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "MessageEn");

            migrationBuilder.AddColumn<string>(
                name: "MessageAr",
                table: "Notifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "Notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageAr",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Pharmacies",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "MessageEn",
                table: "Notifications",
                newName: "Message");
        }
    }
}
