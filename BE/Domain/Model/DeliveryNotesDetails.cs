using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("deliveryNotes_details")]
    public class DeliveryNotesDetails : BaseModel
    {
        [Column("deliveryNotes_id")]

        public long DeliveryNotes_Id { get; set; }

        [ForeignKey(nameof(DeliveryNotes_Id))]
        public DeliveryNotes? DeliveryNotes { get; set; }

        [Column("deliveryNotes_number")]
        public long DeliveryNotesNumber { get; set; }

        [Column("product_id")]
        public long ProductId { get; set; }

        [Column("product_name")]
        public string? ProductName { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

    }
}
