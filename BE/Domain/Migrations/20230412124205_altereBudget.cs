using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class altereBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_cuit",
                table: "budget");

            migrationBuilder.AddColumn<string>(
                name: "payment",
                table: "budget",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$Wgi8is+beJ6cgNAJjav20XLhoq5NuUqIJ1agZOHb1jiVKn2G");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment",
                table: "budget");

            migrationBuilder.AddColumn<string>(
                name: "customer_cuit",
                table: "budget",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$pqzSfkK+rsNKNj87p+1TkrxcAwI/4IgIXnp/mV4lxpTfFagf");
        }
    }
}
