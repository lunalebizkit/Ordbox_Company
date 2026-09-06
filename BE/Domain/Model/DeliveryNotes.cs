using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("delivery_notes")]
    public class DeliveryNotes : BaseModel
    {
        [Column("deliveryNotes_number")]
        public long DeliveryNote_number { get; set; }

        [Required]
        [Column("dateTime")]
        public DateTime DateTime { get; set; }

        [Column("supplier_id")]
        public long SupplierId { get; set; }

        [Column("supplier_name")]
        public string? SupplierName { get; set; }

        [Column("supplier_cuit")]
        public string? SupplierCuit { get; set; }

        [Column("supplier_address")]
        public string? SupplierAddress { get; set; }

        [Column("status_id")]
        public long StatusId { get; set; }

        [Column("cancelled")]
        public string? Cancelled { get; set; }

        [Column("paid")]
        public Boolean Paid { get; set; }

        [Column("observation")]
        public string? Observation { get; set; }

        [Column("import_total")]
        public decimal ImportTotal { get; set; }

        public ICollection<DeliveryNotesDetails> DeliveryNotesDetails { get; set; } = new HashSet<DeliveryNotesDetails>();
    }
}
