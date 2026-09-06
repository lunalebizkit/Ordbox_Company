using Ordbox.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("supplier_order_detail")]
    public class SupplierOrderDetail : BaseModel
    {
        [Column("product_id")]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Column("supplier_order_id")]
        public long SupplierOrderId { get; set; }

        [ForeignKey(nameof(SupplierOrderId))]
        public SupplierOrder SupplierOrder { get; set; }

        [Required]
        [Column("ordered_quantity")]
        public int OrderedQuantity { get; set; }

        [Required]
        [Column("recieved_quantity")]
        public int RecievedQuantity { get; set; }

        [Required]
        [Column("status")]
        public long Status { get; set; }

        [NotMapped]
        public ESupplierOrderStatuses StatusEnum { get => (ESupplierOrderStatuses)Status; set => Status = (int)value; }
    }
}
