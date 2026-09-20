using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class DeliveryNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "delivery_notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deliveryNotes_number = table.Column<long>(type: "bigint", nullable: false),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    supplier_id = table.Column<long>(type: "bigint", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    cancelled = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    import_total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_notes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "deliveryNotes_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deliveryNotes_id = table.Column<long>(type: "bigint", nullable: false),
                    deliveryNotes_number = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliveryNotes_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_deliveryNotes_details_delivery_notes_deliveryNotes_id",
                        column: x => x.deliveryNotes_id,
                        principalTable: "delivery_notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$4WAkgUUlIsbSqHftuLH8vowfh8xlVMuW7CrkigGR6RBTETLv");

            migrationBuilder.CreateIndex(
                name: "IX_deliveryNotes_details_deliveryNotes_id",
                table: "deliveryNotes_details",
                column: "deliveryNotes_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deliveryNotes_details");

            migrationBuilder.DropTable(
                name: "delivery_notes");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$K5WjB1bC/tpy4LO9+m4c0XYQHizxn8rgEbKJA0IcduCo9EZS");
        }
    }
}
