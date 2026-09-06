using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("customer")]
    public class Customer: Entity
    {
        [Column("observation")]
        public string? Observation { get; set; } 
    }
}
