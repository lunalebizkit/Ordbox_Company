using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class ScriptPermissionsXRol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$0C/x+/pSl0/nWmazTYU22nNN5cIC/rgvEGx8hdUWUhwUIfmS");

            migrationBuilder.Sql($"DELETE FROM dbo.permission_x_rol;");
            migrationBuilder.Sql($"INSERT INTO [dbo].[permission_x_rol]" +
                $"([role_id],[permission_id])" +
                $"VALUES  (1 ,1)," +
                $"(1,2)," +
                $"(1,3)," +
                $"(1,4)," +
                $"(1,5)," +
                $"(1,6)," +
                $"(1,7)," +
                $"(1,8)," +
                $"(1,9)," +
                $"(1,10)," +
                $"(1,11)," +
                $"(1,12)," +
                $"(1,13)," +
                $"(1,14)," +
                $"(1,15)," +
                $"(1,16)," +
                $"(1,17)," +
                $"(1,18)," +
                $"(1,19)," +
                $"(1,20)," +
                $"(1,21)," +
                $"(1,22)," +
                $"(1,23)," +
                $"(1,24)," +
                $"(1,25)," +
                $"(1,26)," +
                $"(1,27)," +
                $"(1,28)," +
                $"(1,29)," +
                $"(1,30)," +
                $"(1,31)," +
                $"(1,32)," +
                $"(1,33)," +
                $"(1,34)," +
                $"(1,35)," +
                $"(1,36)," +
                $"(1,37)," +
                $"(1,38)," +
                $"(1,39)," +
                $"(1,40)," +
                $"(1,41)," +
                $"(1,42)," +
                $"(1,43)," +
                $"(1,44)," +
                $"(1,45)," +
                $"(1,46)" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$C4H9hbexQQPW4u6SFH7Oa61oVoDnY8s4eDvNmO8UrdE8GpD0");
        }
    }
}
