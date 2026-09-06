using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("quittance")]
    public class Quittance : BaseModel
    {
        [Column("quittance_number")]
        public int QuittanceNumber { get; set; }

        [Column("customer_name")]
        public string? CustomerName { get; set; }

        [Column("customer_address")]
        public string? Address { get; set; }

        [Column("customer_cuit")]
        public string? CustomerCuit { get; set; }

        [Required]
        [Column("dateTime")]
        public DateTime DateTime { get; set; }
        
        [Column("amount")]
        public string? Amount { get; set; }

        [Column("concept")]
        public string? Concept { get; set; }

        [Column("total")]
        public decimal? Total { get; set; }

        [Column("cash")]
        public decimal? Cash { get; set; }

        public ICollection<QuittanceDetails> QuittanceDetails { get; set; } = new HashSet<QuittanceDetails>();
        public ICollection<QuittanceProductDetails> QuittanceProductDetails { get; set; } = new HashSet<QuittanceProductDetails>();
    }
}
