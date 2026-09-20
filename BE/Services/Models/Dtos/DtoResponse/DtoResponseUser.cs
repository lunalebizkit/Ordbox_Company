namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseUser
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public int RoleId { get; set; }
    }
}
