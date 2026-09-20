
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("category")]
    public class Category : BaseModel
    {
        [Required]
        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

    }
}
