using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetBG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderItems_Additions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemAdditions");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemId",
                table: "Additions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Additions_OrderItemId",
                table: "Additions",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Additions_OrderItems_OrderItemId",
                table: "Additions",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Additions_OrderItems_OrderItemId",
                table: "Additions");

            migrationBuilder.DropIndex(
                name: "IX_Additions_OrderItemId",
                table: "Additions");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Additions");

            migrationBuilder.CreateTable(
                name: "OrderItemAdditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AdditionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AdditionType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemAdditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemAdditions_Additions_AdditionId",
                        column: x => x.AdditionId,
                        principalTable: "Additions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItemAdditions_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAdditions_AdditionId",
                table: "OrderItemAdditions",
                column: "AdditionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAdditions_OrderItemId",
                table: "OrderItemAdditions",
                column: "OrderItemId");
        }
    }
}
