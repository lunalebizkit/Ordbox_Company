namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseCompany
    {
        public long Id { get; set; }

        public required string CompanyName { get; set; }

        public required string CompanyOwnerName { get; set; }

        public required string CompanyCuit { get; set; }

        public string? CompanyAddress { get; set; }

        public required string CompanyEmail { get; set; }

        public required string CompanyEmailPass { get; set; }

        public string? CompanyDescription { get; set; }

        public required short CompanyPoint { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<long> UserIds { get; set; } = [];


    }
}
