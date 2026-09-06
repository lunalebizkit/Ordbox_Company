

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("permission_x_rol")]
    public class PermissionXRol : BaseModel
    {
        [Required]
        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Rol Rol { get; set; }

        [Required]
        [Column("permission_id")]
        public long PermissionId { get; set; }

        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
    }
}
