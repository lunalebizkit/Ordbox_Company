using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("period")]
    public partial class Period : BaseModel
    {
        [Required]
        [Column("init_period")]
        public DateTime InitPeriod { get; set; }

        [Column("end_period")]
        public DateTime EndPeriod { get; set; }

        [Column("status")]
        public bool Status { get; set; }

    }


}
