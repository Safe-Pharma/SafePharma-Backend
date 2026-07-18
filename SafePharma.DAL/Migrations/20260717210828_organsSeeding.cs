using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafePharma.DAL.Migrations
{
    /// <inheritdoc />
    public partial class organsSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganImpairmentLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganImpairmentLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrganFunctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganImpairmentLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrganFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOrganFunctions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrganFunctions_OrganImpairmentLevels_OrganImpairmentLevelId",
                        column: x => x.OrganImpairmentLevelId,
                        principalTable: "OrganImpairmentLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrganFunctions_Organs_OrganId",
                        column: x => x.OrganId,
                        principalTable: "Organs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrganFunctions_CustomerId_OrganId",
                table: "CustomerOrganFunctions",
                columns: new[] { "CustomerId", "OrganId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrganFunctions_OrganId",
                table: "CustomerOrganFunctions",
                column: "OrganId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrganFunctions_OrganImpairmentLevelId",
                table: "CustomerOrganFunctions",
                column: "OrganImpairmentLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrganFunctions");

            migrationBuilder.DropTable(
                name: "OrganImpairmentLevels");

            migrationBuilder.DropTable(
                name: "Organs");
        }
    }
}
