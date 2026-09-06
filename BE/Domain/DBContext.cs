using Ordbox.Domain.Model;
using Ordbox.SDK.Security;
using Microsoft.EntityFrameworkCore;

namespace Ordbox.Domain
{
    public class DBContext : DbContext
    {
        public DBContext()
        {

        }
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Budget>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<BudgetDetail>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<SupplierOrder>()
                .HasMany(i => i.SupplierOrderDetail)
                .WithOne(i => i.SupplierOrder)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Receipt>()
                .HasMany(i => i.ReceiptDetails)
                .WithOne(i => i.Receipt)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Quittance>()
               .HasMany(i => i.QuittanceDetails)
               .WithOne(i => i.Quittance)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Quittance>()
               .HasMany(i => i.QuittanceProductDetails)
               .WithOne(i => i.Quittance)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CreditMemo>()
                .HasMany(i => i.CreditMemoDetail)
                .WithOne(i => i.CreditMemo)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<CreditMemo>()
                .Property(i => i.IvaTotal)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<CreditMemo>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<CreditMemoDetail>()
                .Property(i => i.Iva)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<CreditMemoDetail>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DebitMemo>()
                .Property(i => i.IvaTotal)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DebitMemo>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DebitMemoDetails>()
                .Property(i => i.Iva)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DebitMemoDetails>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DeliveryNotes>()
                .Property(i => i.ImportTotal)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DeliveryNotesDetails>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Invoice>()
                .Property(i => i.IvaTotal)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Invoice>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<InvoiceDetail>()
                .Property(i => i.Iva)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<InvoiceDetail>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Product>()
                .Property(i => i.CardSalePercentage)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Product>()
                .Property(i => i.CashSalePercentage)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Product>()
                .Property(i => i.CardSalePrice)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Product>()
                .Property(i => i.CashSalePrice)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Product>()
                .Property(i => i.PurchasePrice)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Product>()
                .Property(i => i.SalePrice)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Quittance>()
                .Property(i => i.Cash)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Quittance>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<QuittanceDetails>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Receipt>()
                .Property(i => i.ConcNoGravado)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Receipt>()
                .Property(i => i.IvaTotal)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Receipt>()
                .Property(i => i.PercIngBrutos)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Receipt>()
                .Property(i => i.PercIva)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<Receipt>()
                .Property(i => i.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<ReceiptDetails>()
                .Property(i => i.Iva)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<ReceiptDetails>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<ReceiptDetails>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Rol>().HasData(
               new Rol()
               {
                   Id = 1,
                   Key = "1",
                   Name = "Admin",
               });

            builder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    FirstName = "admin",
                    LastName = "admin",
                    UserName = "admin",
                    Email = "admin",
                    Password = SecurePasswordHasher.Hash("admin123", 100),
                    IsDeleted = false,
                    RoleId = 1,
                    CompanyId = null
                });
            builder.Entity<Customer>().HasData(
               new Customer()
               {
                   Id = 1,
                   Name = "admin",
                   Dni = 0,
                   Address = "S/N",
                   Cuit = "0"
               });

            base.OnModelCreating(builder);
        }

        public virtual DbSet<Rol> Rols { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionXRol> PermissionXRols { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<EmailEntity> EmailEntities { get; set; }
        public virtual DbSet<PhoneEntity> PhoneEntities { get; set; }
        public virtual DbSet<SupplierOrder> SupplierOrders { get; set; }
        public virtual DbSet<SupplierOrderDetail> SupplierOrderDetails { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<ReceiptDetails> ReceiptDetails { get; set; }
        public virtual DbSet<CreditMemo> CreditMemo { get; set; }
        public virtual DbSet<CreditMemoDetail> CreditMemoDetail { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<DebitMemo> DebitMemos { get; set; }
        public virtual DbSet<DebitMemoDetails> DebitMemoDetails { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<BudgetDetail> BudgetDetails { get; set; }
        public virtual DbSet<DeliveryNotes> DeliveryNotes { get; set; }
        public virtual DbSet<DeliveryNotesDetails> DeliveryNotesDetails { get; set; }
        public virtual DbSet<Quittance> Quittance { get; set; }
        public virtual DbSet<QuittanceDetails> QuittanceDetails { get; set; }
        public virtual DbSet<QuittanceProductDetails> QuittanceProductDetails { get; set; }
        public virtual DbSet<InvoiceSPReport> InvoiceSPReports { get; set; }
        public virtual DbSet<InvoiceSPReportTotal> InvoiceSPReportTotals { get; set; }
        public virtual DbSet<IntegrationLog> IntegrationLogs { get; set; }
        public virtual DbSet<IntegrationLogInvoice> IntegrationLogInvoices { get; set; }
        public virtual DbSet<AuthRefresh> AuthRefreshes { get; set; }

        public virtual DbSet<Company> Companies { get; set; }
    }
}
