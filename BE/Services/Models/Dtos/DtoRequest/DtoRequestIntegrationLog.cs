using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestIntegrationLog
    {
        public long Id { get; set; }
        public long? UniqueId { get; set; }
        public DateTime? GenerationTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string Token { get; set; }
        public string Sign { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Endpoint { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
