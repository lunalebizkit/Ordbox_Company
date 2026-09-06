using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Domain.Model
{
    [Table("debit_memo")]
    public class DebitMemo : BaseModel
    {
        [Column("invoice_id")]
        public Nullable<long> InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice? Invoice { get; set; }

        [Column("debitMemo_number")]
        public long DebitMemoNumber { get; set; }

        [Required]
        [Column("dateTime")]
        public DateTime DateTime { get; set; }

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

        [Column("total")]
        public decimal Total { get; set; }

        [Column("iva_total")]
        public decimal IvaTotal { get; set; }

        [Column("type")]
        public int Type { get; set; }
        public ICollection<DebitMemoDetails> DebitMemoDetails { get; set; } = new HashSet<DebitMemoDetails>();
    }
   
       
}
