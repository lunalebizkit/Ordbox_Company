
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ordbox.Domain.Model
{
    [Table("permission")]
    public class Permission : BaseModel
    {
        [Required]
        [Column("name")]
        public string? Name { get; set; }

        [Column("key")]
        public string? Key { get; set; }

        [Column("enumPermission")]
        public int EnumPermission { get; set; }
        public List<PermissionXRol> PermissionXRols { get; set; }
    }
}
