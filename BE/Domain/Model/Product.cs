using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("product")]
    public partial class Product : BaseModel
    {
        [Required]
        [Column("description")]
        public string? Description { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("category_id")]
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Column("brand_id")]
        public long BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; } //cantidad

        [Column("purchase_price")]
        public decimal PurchasePrice { get; set; } //precio de compra "Costo"

        [Column("sale_price")]
        public decimal SalePrice { get; set; } //precio de venta (lista)

        [Column("sale_percentage")]
        public int SalePercentage { get; set; } //porcentaje de venta (lista)

        [Column("card_sale_price")]
        public decimal CardSalePrice { get; set; } //precio con tarjeta

        [Column("card_sale_percentage")]
        public decimal CardSalePercentage { get; set; } //porcentaje de tarjeta

        [Column("cash_sale_price")]
        public decimal CashSalePrice { get; set; } //precio con contado

        [Column("cash_sale_percentage")]
        public decimal CashSalePercentage { get; set; } //porcentaje de contado

        [Column("point_order")]
        public int PointOrder { get; set; } //punto de pedido

        [Column("observation")]
        public string? Observation { get; set; } //Observaciones
                                                 
        [Column("bar_code")]
        public string? BarCode { get; set; } //Observaciones

        [Column("supplier_id")]
        public long SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

    }
}
