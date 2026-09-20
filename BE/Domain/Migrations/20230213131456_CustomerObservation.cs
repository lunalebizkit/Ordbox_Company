using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class CustomerObservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_customer_customer_id",
                table: "invoice");

            migrationBuilder.AlterColumn<long>(
                name: "customer_id",
                table: "invoice",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "debitMemo_number",
                table: "debit_memo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "observation",
                table: "customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_customer_customer_id",
                table: "invoice",
                column: "customer_id",
                principalTable: "customer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_customer_customer_id",
                table: "invoice");

            migrationBuilder.DropColumn(
                name: "debitMemo_number",
                table: "debit_memo");

            migrationBuilder.DropColumn(
                name: "observation",
                table: "customer");

            migrationBuilder.AlterColumn<long>(
                name: "customer_id",
                table: "invoice",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_customer_customer_id",
                table: "invoice",
                column: "customer_id",
                principalTable: "customer",
                principalColumn: "id");
        }
    }
}
