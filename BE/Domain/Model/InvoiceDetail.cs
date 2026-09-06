
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("invoice_detail")]
    public partial class InvoiceDetail : BaseModel
    {
        [Column("invoice_id")]
        public long InvocieId { get; set; }

        [ForeignKey(nameof(InvocieId))]
        public Invoice? Invoice { get; set; }

        [Column("product_id")]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Column("product_name")]
        public string? ProductName { get; set; }

        [Column("product_code")]
        public string? ProductCode { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("iva")]
        public decimal Iva { get; set; }
    }
}
