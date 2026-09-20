using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("auth_refresh")]
    public class AuthRefresh : BaseModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("token_hash")]
        public string TokenHash { get; set; }

        [Column("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }

        [Column("revoked_at")]
        public DateTimeOffset? RevokedAt { get; set; }

        [Column("created_on")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}
