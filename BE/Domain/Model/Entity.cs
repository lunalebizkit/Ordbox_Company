using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Domain.Model
{
    [Table("entity")]
    public partial class Entity : BaseModel
    {
        [Column("dni")]
        public int? Dni { get; set; }

        [Column("cuit")]
        public string? Cuit { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("isInactive")]
        public bool IsInactive { get; set; }


        public string EmailsStrings { 
            get
            {
                return EmailEntities.Count == 0? String.Empty : EmailEntities.Select(p => p.Email).Aggregate((a, b) => $"{a} - {b}");
            }
        }

        public string PhoneString
        {
            get
            {
                return PhoneEntities.Count == 0 ? String.Empty : PhoneEntities.Select(p => p.PhoneNumber).Aggregate((a, b) => $"{a} - {b}");
            }
        }

        public ICollection<PhoneEntity> PhoneEntities { get; set; } = new HashSet<PhoneEntity>();

        public ICollection<EmailEntity> EmailEntities { get; set; } = new HashSet<EmailEntity>();

    }
}
