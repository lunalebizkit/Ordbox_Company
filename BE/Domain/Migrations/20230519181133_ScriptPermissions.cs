using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordbox.Domain.Migrations
{
    public partial class ScriptPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$C4H9hbexQQPW4u6SFH7Oa61oVoDnY8s4eDvNmO8UrdE8GpD0");
           
            migrationBuilder.Sql($"DELETE FROM dbo.permission");
            migrationBuilder.Sql($"DBCC CHECKIDENT ('dbo.permission', RESEED,0)");
            migrationBuilder.Sql($"INSERT INTO [dbo].[permission]" +
                $"([name]" +
                $",[key]" +
                $",[enumPermission])" +
                $" VALUES" +
                $"('VerUsuario','ViewUser',1)," +
                $"('EditarUsuario', 'EditUser',2)," +
                $"('EliminarUsuario', 'DeleteUser',3)," +
                $"('CrearUsuario', 'CreateUser',4)," +
                $"('VerMarca','ViewBrand',5)," +
                $"('CrearMarca','CreateBrand',6)," +
                $"('EditarMarca','EditBrand',7)," +
                $"('VerCategoria','ViewCategory',8)," +
                $"('CrearCategoria','CreateCategory',9)," +
                $"('EditarCategoria','EditCategory',10)," +
                $"('VerCliente','ViewCustomer',11)," +
                $"('CrearCliente','CreateCustomer',12)," +
                $"('EditarCliente','EditCustomer',13)," +
                $"('VerEntidades','ViewEntity',14)," +
                $"('CrearEntidades','CreateEntity',15)," +
                $"('EditarEntidades','EditEntity',16)," +
                $"('ListadoFactura','GetInvoice',17)," +
                $"('CrearFactura','CreateInvoice',18)," +
                $"('VerProducto','ViewProduct',19)," +
                $"('CrearProducto','CreateProduct',20)," +
                $"('EditarProducto','EditProduct',21)," +
                $"('VerProveedor','ViewSupplier',22)," +
                $"('EditarProveedor','EditSupplier',23)," +
                $"('CrearProveedor','CreateSupplier',24)," +
                $"('VerPedidos','ViewOrderSupplier',25)," +
                $"('CrearPedido','CreateOrderSupplier',26)," +
                $"('EditarPedido','EditOrderSupplier',27)," +
                $"('ListaActualizarPrecio','ListUpdatePrice',28)," +
                $"('ActualizarPrecio','EditUpdatePrice',29)," +
                $"('ListadoFacturaCompra','GetReceipt',30), " +
                $"('CrearFacturaCompra','CreateReceipt',31)," +
                $"('ListadoIva','ListIva',32)," +
                $"('ListadoPeriodo','GetPeriod',33)," +
                $"('CrearPeriodo','CreatePeriod',34)," +
                $"('EditarPeriodo','EditPeriod',35)," +
                $"('EliminarPeriodo','DeletePeriod',36)," +
                $"('ListadoNotaCreditoYDebito','GetMemo', 37)," +
                $"('CrearNotaCreditoYDebito','CreateMemo',38)," +
                $"('ControldeRoles','RolControl',39)," +
                $"('ReportZ','ReportZ', 40)," +
                $"('ListadoPresupuesto','GetBudget', 41)," +
                $"('CrearPresupuesto','CreateBudget', 42)," +
                $"('ListadoRemito','GetRemito', 43)," +
                $"('CrearRemito','CreateRemito', 44)," +
                $"('ListadoRecibos','GetQuittance',45)," +
                $"('CrearRecibos','CreateQuittance',46)");
    
        }
       
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$MYHASH$V1$100$x4irwZ93oLGN4UKQbUKn/Gv+flHU48USvAnd5MRqrXxmvRrF");
        }
    }
}
