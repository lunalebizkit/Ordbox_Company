namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoEntity
    {
        public long Id { get; set; }

        public int? Dni { get; set; }

        public string? Cuit { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public List<string> PhoneEntity { get; set; }

        public List<string> EmailEntity { get; set; }
        public string? Observation { get; set; }
    }
}
