
namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseProduct
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string? Code { get; set; }

        public string CategoryName { get; set; }

        public string BrandName { get; set; }

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePercentage { get; set; }

        public decimal SalePrice { get; set; }

        public decimal CardSalePercentage { get; set; }

        public decimal CardSalePrice { get; set; }

        public decimal CashSalePercentage { get; set; }

        public decimal CashSalePrice { get; set; }

        public int PointOrder { get; set; }

        public string Observation { get; set; }
        public string BarCode { get; set; }

        public string SupplierName { get; set; }
        public bool IsDeleted { get; set; }
        public List<DtoGenericResponse> Category { get; set; }
        public List<DtoGenericResponse> Brand { get; set; }
        public List<DtoGenericResponse> Supplier { get; set; }
    }
}
