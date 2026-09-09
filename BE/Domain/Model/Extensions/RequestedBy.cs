using Ordbox.Domain.Enum;

namespace Ordbox.Domain.Model.Extensions
{
    public class RequestedBy
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public ERol UserRolId { get; set; }

        public long CompanyId { get; set; }
    }
}
