namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseIntegrationLogCredit
    {
        public int Id { get; set; }
        public int CreditId { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string EndPoint { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
