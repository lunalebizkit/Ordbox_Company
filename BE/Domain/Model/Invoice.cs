using Ordbox.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("invoice")]
    public partial class Invoice : BaseModel
    {       
        [Column("customer_id")]
        public long? CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Column("invoice_number")]
        public long InvoiceNumber { get; set; }

        [Column("customer_name")]
        public string? CustomerName { get; set; }

        [Column("customer_cuit")]
        public string? CustomerCuit { get; set; }

        [Column("customer_address")]
        public string? CustomerAddress { get; set; }

        [Column("observation")]
        public string? Observation { get; set; }

        [Required]
        [Column("dateTime")]
        public DateTime DateTime { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("iva_total")]
        public decimal IvaTotal { get; set; }

        [Column("type")]
        public int Type { get; set; }
        
        [Column("cae")]
        public string? CAE { get; set; }

        [Column("cae_expiration_date")]
        public DateTime? CAEExpirationDate { get; set; }
        
        [Column("integration_success")]
        public bool? IntegrationSuccess { get; set; }
        
        [Column("version")]
        public byte Version { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new HashSet<InvoiceDetail>();


        [NotMapped]
        public ETypeReceipt Status { get => (ETypeReceipt)Type; }
    }
}
