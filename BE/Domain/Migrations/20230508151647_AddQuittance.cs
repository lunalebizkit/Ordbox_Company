using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class AddQuittance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quittance",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quittance_number = table.Column<int>(type: "int", nullable: false),
                    customer_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_cuit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quittance", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quittance_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    total = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    check_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quittance_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quittance_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_quittance_details_quittance_quittance_id",
                        column: x => x.quittance_id,
                        principalTable: "quittance",
                        principalColumn: "id");
                });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$xzTyVaguwTx0pf8xIUMSDYVVuxMbNiWUUcCZ7sIgYqPegS/8");

            migrationBuilder.CreateIndex(
                name: "IX_quittance_details_quittance_id",
                table: "quittance_details",
                column: "quittance_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quittance_details");

            migrationBuilder.DropTable(
                name: "quittance");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$vo/f4mtcHntqKdXuI7ZbBXPX+ng1joOAFiW8fPJD6ITgQTqO");
        }
    }
}
