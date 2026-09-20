
using Ordbox.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("receipt")]
    public class Receipt : BaseModel
    {
        [Column("supplier_id")]
        public long? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Column("receipt_number")]
        public int ReceiptNumber { get; set; }

        [Column("supplier_name")]
        public string? SupplierName { get; set; }

        [Column("supplier_cuit")]
        public string? SupplierCuit { get; set; }

        [Column("supplier_address")]
        public string? SupplierAddress { get; set; }

        [Column("observation")]
        public string? Observation { get; set; }

        [Column("dateTime")]
        public DateTime DateTime { get; set; }

        [Column("total")]
        public decimal? Total { get; set; }

        [Column("iva_total")]
        public decimal IvaTotal { get; set; }


        [Column("conc_no_gravado")]
        public decimal ConcNoGravado { get; set; }


        [Column("perc_iva")]
        public decimal PercIva { get; set; }


        [Column("perc_ing_brutos")]
        public decimal PercIngBrutos { get; set; }

        [Column("type")]
        public int Type { get; set; }

        [Column("is_inactive")]
        public bool IsInactive { get; set; }

        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public ICollection<ReceiptDetails> ReceiptDetails { get; set; } = new HashSet<ReceiptDetails>();

        [NotMapped]
        public ETypeReceipt Status { get => (ETypeReceipt)Type; }

    }
}
