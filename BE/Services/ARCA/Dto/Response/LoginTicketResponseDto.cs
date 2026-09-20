namespace Ordbox.Services.ARCA.Dto.Response
{
    public class LoginTicketResponseDto
    {
        public long UniqueId { get; set; }
        public DateTime? GenerationTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Sign { get; set; } = string.Empty;
        public string XmlRequest { get; set; } = string.Empty;
        public string XmlResponse { get; set; } = string.Empty;
    }
}
