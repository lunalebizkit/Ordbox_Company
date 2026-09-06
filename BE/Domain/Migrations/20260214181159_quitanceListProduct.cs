using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class quitanceListProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "quittance_product_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quittance_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    product_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    iva = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quittance_product_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_quittance_product_details_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quittance_product_details_quittance_quittance_id",
                        column: x => x.quittance_id,
                        principalTable: "quittance",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$hBxczFrtu1QZV7Px1deJIeiBTt5XwDYCi3m8CNuXNXrnz8DK");

            migrationBuilder.CreateIndex(
                name: "IX_quittance_product_details_product_id",
                table: "quittance_product_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_quittance_product_details_quittance_id",
                table: "quittance_product_details",
                column: "quittance_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quittance_product_details");

            migrationBuilder.DropColumn(
                name: "is_inactive",
                table: "receipt");

            migrationBuilder.DropColumn(
                name: "is_inactive",
                table: "budget");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$xm8aK0dEnyihoSeouDnmF/Ang0GWAcM1KNAIQIfwCLwHzeXO");
        }
    }
}
