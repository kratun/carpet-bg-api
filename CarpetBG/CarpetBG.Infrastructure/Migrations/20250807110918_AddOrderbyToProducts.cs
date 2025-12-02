using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetBG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderbyToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderBy",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderBy",
                table: "Products");
        }
    }
}
