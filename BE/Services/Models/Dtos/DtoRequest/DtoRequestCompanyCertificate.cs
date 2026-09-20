using Microsoft.AspNetCore.Http;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestCompanyCertificate
    {
        public long CompanyId { get; set; }

        public IFormFile CertificateData { get; set; }

        public required string Password { get; set; }
    }
}
