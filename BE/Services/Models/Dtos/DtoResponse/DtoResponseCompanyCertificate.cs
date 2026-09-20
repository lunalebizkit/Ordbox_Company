namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseCompanyCertificate
    {
        public long CompanyId { get; set; }
        public byte[] CertificateData { get; set; }
        public byte[] PasswordEncrypted { get; set; }

        public bool IsActive { get; set; }
    }
}
