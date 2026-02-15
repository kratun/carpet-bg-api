using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetBG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePichupDateAndDeliveryDateTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "pickup_date_new",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "delivery_date_new",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            // FORCE MIDNIGHT UTC
            migrationBuilder.Sql(@"
                UPDATE orders
                SET pickup_date_new =
                    (pickup_date::timestamp AT TIME ZONE 'UTC')
                WHERE pickup_date IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE orders
                SET delivery_date_new =
                    (delivery_date::timestamp AT TIME ZONE 'UTC')
                WHERE delivery_date IS NOT NULL;
            ");

            migrationBuilder.DropColumn(name: "pickup_date", table: "orders");
            migrationBuilder.DropColumn(name: "delivery_date", table: "orders");

            migrationBuilder.RenameColumn(
                name: "pickup_date_new",
                table: "orders",
                newName: "pickup_date");

            migrationBuilder.RenameColumn(
                name: "delivery_date_new",
                table: "orders",
                newName: "delivery_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "pickup_date",
                table: "orders",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "delivery_date",
                table: "orders",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
