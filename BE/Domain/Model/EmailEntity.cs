using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Domain.Model
{
    [Table("email_entity")]
    public class EmailEntity : BaseModel
    {
        [Column("entity_id")]
        public long EntityId { get; set; }

        [ForeignKey("EntityId")]
        public Entity Entity { get; set; }

        [Column("email")]
        public string Email { get; set; }
    }
}
