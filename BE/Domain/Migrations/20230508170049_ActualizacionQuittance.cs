using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class ActualizacionQuittance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "quittance_details",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "quittance_details",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "quittance",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "cash",
                table: "quittance",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$6dOq2rSiqzHlJKV7cEZGaWk9fYTIicmO5HSihRbRPfg6YgG+");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "quittance_details");

            migrationBuilder.DropColumn(
                name: "cash",
                table: "quittance");

            migrationBuilder.AlterColumn<string>(
                name: "total",
                table: "quittance_details",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "total",
                table: "quittance",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$yshzptcBGHgPtWdmlhlCjZHWP3f+SjlszuWSnspgR89Q2oHm");
        }
    }
}
