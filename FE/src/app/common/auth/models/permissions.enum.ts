/** @enum: Permisos de roles*/
export enum Permission {
  //User
  ViewUser = 1,
  EditUser = 2,
  DeleteUser = 3,
  CreateUser = 4,
  //Brand
  ViewBrand = 5,
  CreateBrand = 6,
  EditBrand = 7,
  DeleteBrand = 47,
  //Category
  ViewCategory = 8,
  CreateCategory = 9,
  EditCategory = 10,
  //Customer
  ViewCustomer = 11,
  CreateCustomer = 12,
  EditCustomer = 13,
  //Entity
  ViewEntity = 14,
  CreateEntity = 15,
  EditEntity = 16,
  //Invoice
  GetInvoice = 17,
  CreateInvoice = 18,
  //Product
  ViewProduct = 19,
  CreateProduct = 20,
  EditProduct = 21,
  DeleteProduct = 48,
  //Supplier
  ViewSupplier = 22,
  EditSupplier = 23,
  CreateSupplier = 24,
  //OrderSupplier
  ViewOrderSupplier = 25,
  CreateOrderSupplier = 26,
  EditOrderSupplier = 27,
  //UpdatePrice
  ListUpdatePrice = 28,
  EditUpdatePrice = 29,
  //Receipt
  GetReceipt = 30,
  CreateReceipt = 31,
  //IVA
  ListIva = 32,
  //Crear periodo
  GetPeriod = 33,
  CreatePeriod = 34,
  EditPeriod = 35,
  DeletePeriod = 36,
  GetMemo = 37,
  CreateMemo=38,
  RolControl= 39,
  ReportZ=40,
  //Budget
  GetBudget=41,
  CreateBudget=42,
  //Remito
  GetRemito = 43,
  CreateRemito = 44,
  //Recibo
  GetQuittance= 45,
  CreateQuittance=46
}
