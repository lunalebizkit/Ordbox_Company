using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("customer")]
    public class Customer: Entity
    {
        [Column("observation")]
        public string? Observation { get; set; }

        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
