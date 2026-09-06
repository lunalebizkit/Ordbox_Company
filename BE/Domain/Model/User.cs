

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("user")]
    public class User : BaseModel
    {

        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("user_name")]
        public required string UserName { get; set; }

        [Required]
        [MaxLength(250)]
        [Column("password")]
        public required string Password { get; set; }

        [MaxLength(250)]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Required]
        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Rol Rol { get; set; }

        [Column("company_id")]
        public long? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}
