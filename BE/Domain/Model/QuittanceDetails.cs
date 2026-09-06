using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("quittance_details")]
    public class QuittanceDetails : BaseModel
    {
        [Column("quittance_id")]
        public long QuittanceId { get; set; }

        [ForeignKey("QuittanceId")]
        public Quittance? Quittance { get; set; }

        [Column("total")]
        public decimal? Total { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("bank")]
        public string? Bank { get; set; }

        [Column("check_number")]
        public string? CheckNumber { get; set; }
    }
}
