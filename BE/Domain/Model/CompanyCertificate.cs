

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("company_certificate")]
    public class CompanyCertificate : BaseModel
    {      
        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        [Required]
        [Column("certificate_data")]
        public byte[] CertificateData { get; set; } = [];

        [Column("password_encrypted")]
        public byte[]? PasswordEncrypted { get; set; }

        [Required]
        [Column("fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime? FechaActualizacion { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; }

        [Required]
        [Column("algoritmo_cifrado")]
        public string AlgoritmoCifrado { get; set; } = "AES256";

        [Required]
        [Column("guid_unico")]
        public Guid GuidUnico { get; set; }

    }
}
