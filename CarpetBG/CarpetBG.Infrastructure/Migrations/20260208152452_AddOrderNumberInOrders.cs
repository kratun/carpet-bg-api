using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetBG.Infrastructure.Migrations
{
    public partial class AddOrderNumberInOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Create sequence (reserve space for future import)
            migrationBuilder.Sql(@"
                CREATE SEQUENCE order_number_seq
                AS BIGINT
                START WITH 100000
                INCREMENT BY 1
                NO MINVALUE
                NO MAXVALUE
                CACHE 1;
            ");

            // 2️⃣ Add column nullable (required for existing rows)
            migrationBuilder.AddColumn<long>(
                name: "order_number",
                table: "orders",
                type: "bigint",
                nullable: true);

            // 3️⃣ Default value for NEW orders
            migrationBuilder.Sql(@"
                ALTER TABLE orders
                ALTER COLUMN order_number
                SET DEFAULT nextval('order_number_seq');
            ");

            // 4️⃣ (Performance) index to speed ordered backfill
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_orders_created_at
                ON orders (created_at);
            ");

            // 5️⃣ Backfill EXISTING orders in chronological order
            migrationBuilder.Sql(@"
                WITH numbered AS (
                    SELECT 
                        id,
                        nextval('order_number_seq') AS new_order_number
                    FROM (
                        SELECT id
                        FROM orders
                        WHERE order_number IS NULL
                        ORDER BY created_at ASC, id ASC
                    ) ordered
                )
                UPDATE orders o
                SET order_number = n.new_order_number
                FROM numbered n
                WHERE o.id = n.id;
            ");

            // 6️⃣ Now enforce NOT NULL
            migrationBuilder.AlterColumn<long>(
                name: "order_number",
                table: "orders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // 7️⃣ Unique business constraint (critical safety net)
            migrationBuilder.CreateIndex(
                name: "ux_orders_order_number",
                table: "orders",
                column: "order_number",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_orders_order_number",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_created_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "order_number",
                table: "orders");

            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS order_number_seq;");
        }
    }
}
