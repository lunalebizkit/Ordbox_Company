using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class deliveryNotesActualizado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "product_name",
                table: "deliveryNotes_details",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "supplier_address",
                table: "delivery_notes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "supplier_cuit",
                table: "delivery_notes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "supplier_name",
                table: "delivery_notes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$iURZd9ts4+Be4eRDioAxdWdYG7rdbVTxMfS/75RFWtby4riI");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_name",
                table: "deliveryNotes_details");

            migrationBuilder.DropColumn(
                name: "supplier_address",
                table: "delivery_notes");

            migrationBuilder.DropColumn(
                name: "supplier_cuit",
                table: "delivery_notes");

            migrationBuilder.DropColumn(
                name: "supplier_name",
                table: "delivery_notes");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$4WAkgUUlIsbSqHftuLH8vowfh8xlVMuW7CrkigGR6RBTETLv");
        }
    }
}
