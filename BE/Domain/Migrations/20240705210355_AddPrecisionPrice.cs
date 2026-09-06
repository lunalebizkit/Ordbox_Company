using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class AddPrecisionPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$9fXvHjin0nBk2qQhKUlxIY8wWXk45N5hq7RNRkGLIy9VMbec");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
