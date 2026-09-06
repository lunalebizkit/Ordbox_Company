using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class ediciondeliveryNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "paid",
                table: "delivery_notes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "payment",
                table: "budget",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$ifoNhWy8pNHbtkz/DqH8kpYCbwEX85C3dIdXDLflP20rpzHw");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "paid",
                table: "delivery_notes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "payment",
                table: "budget",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$iURZd9ts4+Be4eRDioAxdWdYG7rdbVTxMfS/75RFWtby4riI");
        }
    }
}
