using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("integration_log_debit")]
    public class IntegrationLogDebit : BaseModel
    {
        [Column("debit_id")]
        public long? DebitId { get; set; }

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
