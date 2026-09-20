using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("integration_log")]
    public class IntegrationLog : BaseModel
    {
        [Column("unique_id")]
        public long? UniqueId { get; set; }

        [Column("generation_time")]
        public DateTime? GenerationTime { get; set; }

        [Column("expiration_time")]
        public DateTime? ExpirationTime { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("sign")]
        public string Sign { get; set; }

        [Column("request")]
        public string Request { get; set; }

        [Column("response")]
        public string Response { get; set; }

        [Column("endpoint")]
        public string Endpoint { get; set; }

        [Column("success")]
        public bool Success { get; set; }

        [Column("created_on")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}
