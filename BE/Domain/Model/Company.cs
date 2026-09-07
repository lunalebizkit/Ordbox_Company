using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("company")]
    public class Company : BaseModel
    {
        [Required]
        [Column("company_name")]
        public required string CompanyName { get; set; }
        
        [Required]
        [Column("company_ownername")]
        public required string CompanyOwnerName { get; set; }
        
        [Required]
        [Column("company_cuit")]
        public required string CompanyCuit { get; set; }
        
        [Column("company_address")]
        public string? CompanyAddress { get; set; }

        [Required]
        [Column("company_email")]
        public required string CompanyEmail { get; set; }
        
        [Required]
        [Column("company_email_pass")]
        public required string CompanyEmailPass { get; set; }

        [Column("company_description")]
        public string? CompanyDescription { get; set; }

        [Required]
        [Column("company_point")]
        public required short CompanyPoint { get; set; }

        [Required]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public ICollection<User> Users { get; set; } = [];

    }
}
