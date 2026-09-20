using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class createIntegrationLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quittance_product_details_quittance_quittance_id",
                table: "quittance_product_details");

            migrationBuilder.CreateTable(
                name: "integration_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unique_id = table.Column<long>(type: "bigint", nullable: true),
                    generation_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    expiration_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    xml_request = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    xml_response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    endpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integration_log", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$Yqrhq/5PldL+jXQmXJWWPQU7TAlB0RkiJwZsn6MMYDaiGW3M");

            migrationBuilder.AddForeignKey(
                name: "FK_quittance_product_details_quittance_quittance_id",
                table: "quittance_product_details",
                column: "quittance_id",
                principalTable: "quittance",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quittance_product_details_quittance_quittance_id",
                table: "quittance_product_details");

            migrationBuilder.DropTable(
                name: "integration_log");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$hBxczFrtu1QZV7Px1deJIeiBTt5XwDYCi3m8CNuXNXrnz8DK");

            migrationBuilder.AddForeignKey(
                name: "FK_quittance_product_details_quittance_quittance_id",
                table: "quittance_product_details",
                column: "quittance_id",
                principalTable: "quittance",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
