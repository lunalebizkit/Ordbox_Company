using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "rol",
                columns: new[] { "id", "key", "name" },
                values: new object[] { 1L, "1", "Admin" });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "email", "first_name", "is_deleted", "last_name", "password", "role_id", "user_name" },
                values: new object[] { 1L, "admin", "admin", false, "admin", "$MYHASH$V1$100$Z39YCTBReLfqWZVX0AAyMjbNcce5nDWHaB5ipf7wv+tbmfz6", 1L, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "rol",
                keyColumn: "id",
                keyValue: 1L);
        }
    }
}
