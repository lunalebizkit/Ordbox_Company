using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("integration_log_invoice")]
    public class IntegrationLogInvoice : BaseModel
    {
        [Column("invoice_id")]
        public long? InvoiceId { get; set; }

        [Column("request")]
        public string? Request { get; set; }

        [Column("response")]
        public string? Response { get; set; }

        [Column("endpoint")]
        public string Endpoint { get; set; }

        [Column("success")]
        public bool Success { get; set; }

        [Column("created_on")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}
