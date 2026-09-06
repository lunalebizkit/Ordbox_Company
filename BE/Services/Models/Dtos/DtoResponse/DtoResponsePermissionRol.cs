

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponsePermissionRol
    {
        public long Id { get; set; }
        public string Rol { get; set; }
        public List<DtoResponsePermission> Permissions { get; set; }
    }
}
