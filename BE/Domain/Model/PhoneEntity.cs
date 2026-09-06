using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Domain.Model
{
    [Table("phone_entity")]
    public class PhoneEntity : BaseModel
    {
        [Column("entity_id")]
        public long EntityId { get; set; }

        [ForeignKey("EntityId")]
        public Entity Entity { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }
    }
}
