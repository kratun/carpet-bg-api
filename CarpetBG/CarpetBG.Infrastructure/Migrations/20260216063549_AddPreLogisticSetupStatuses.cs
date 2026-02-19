using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetBG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPreLogisticSetupStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_orders_status",
                table: "orders");

            migrationBuilder.AddCheckConstraint(
                name: "ck_orders_status",
                table: "orders",
                sql: "status IN ('new', 'prepickupsetup', 'pendingpickup', 'pickupcomplete', 'washinginprogress', 'washingcomplete', 'predeliverysetup', 'pendingdelivery', 'personaldelivery', 'deliverycomplete', 'cancelled', 'completed')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_orders_status",
                table: "orders");

            migrationBuilder.AddCheckConstraint(
                name: "ck_orders_status",
                table: "orders",
                sql: "status IN ('new', 'pendingpickup', 'pickupcomplete', 'washinginprogress', 'washingcomplete', 'personaldelivery', 'pendingdelivery', 'deliverycomplete', 'cancelled', 'completed')");
        }
    }
}
