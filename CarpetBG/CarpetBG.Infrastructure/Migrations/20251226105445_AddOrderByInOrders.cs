using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetBG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderByInOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderBy",
                table: "Orders",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderBy",
                table: "Orders");
        }
    }
}
