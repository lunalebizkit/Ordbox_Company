using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class customerAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "entity",
                columns: new[] { "id", "address", "cuit", "dni", "name" },
                values: new object[] { 1L, "S/N", "0", 0, "admin" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$K5WjB1bC/tpy4LO9+m4c0XYQHizxn8rgEbKJA0IcduCo9EZS");

            migrationBuilder.InsertData(
                table: "customer",
                column: "id",
                value: 1L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "customer",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "entity",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$Z39YCTBReLfqWZVX0AAyMjbNcce5nDWHaB5ipf7wv+tbmfz6");
        }
    }
}
