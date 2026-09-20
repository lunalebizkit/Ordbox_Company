using Ordbox.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("supplier_order")]
    public class SupplierOrder : BaseModel
    {
     
        [Column("supplier_id")]
        public long SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [Column("supplier_order_number")]
        public long SupplierOrderNumber { get; set; }

        [Column("is_paid")]
        public bool IsPaid { get; set; }

        [Column("observation")]
        public string? Observation { get; set; }

        [Required]
        [Column("status_id")]
        public long StatusId { get; set; }

        [Required]
        [Column("dateTime")]
        public DateTime DateTime { get; set; }

        [Required]
        [Column("scheduled_date")]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public ICollection<SupplierOrderDetail> SupplierOrderDetail { get; set; } = new HashSet<SupplierOrderDetail>();

        [NotMapped]
        public ESupplierOrderStatuses Status { get => (ESupplierOrderStatuses)StatusId; }
    }
}
