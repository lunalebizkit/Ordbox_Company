

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestAddPermissionXRol
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Key { get; set; }

        public List<long>? PermissionIds { get; set; }

    }
}
