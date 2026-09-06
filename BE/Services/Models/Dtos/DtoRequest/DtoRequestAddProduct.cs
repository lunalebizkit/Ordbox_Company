using Ordbox.Services.Common;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestAddProduct
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string? Code { get; set; }

        public long CategoryId { get; set; }

        public long BrandId { get; set; }

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public decimal SalePercentage { get; set; }

        public decimal CardSalePrice { get; set; }

        public decimal CardSalePercentage { get; set; }

        public decimal CashSalePrice { get; set; }

        public decimal CashSalePercentage { get; set; }

        public int PointOrder { get; set; }

        public string Observation { get; set; }
        public string BarCode { get; set; }

        public long SupplierId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
