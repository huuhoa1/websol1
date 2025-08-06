using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace webapp1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedDate", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 7, 7, 10, 23, 38, 175, DateTimeKind.Local).AddTicks(9006), true, "Laptop", 999.99m },
                    { 2, "Electronics", new DateTime(2025, 7, 12, 10, 23, 38, 178, DateTimeKind.Local).AddTicks(1580), true, "Mouse", 29.99m },
                    { 3, "Electronics", new DateTime(2025, 7, 17, 10, 23, 38, 178, DateTimeKind.Local).AddTicks(1607), true, "Keyboard", 79.99m },
                    { 4, "Electronics", new DateTime(2025, 7, 22, 10, 23, 38, 178, DateTimeKind.Local).AddTicks(1611), true, "Monitor", 299.99m },
                    { 5, "Furniture", new DateTime(2025, 7, 27, 10, 23, 38, 178, DateTimeKind.Local).AddTicks(1614), true, "Desk Chair", 149.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
