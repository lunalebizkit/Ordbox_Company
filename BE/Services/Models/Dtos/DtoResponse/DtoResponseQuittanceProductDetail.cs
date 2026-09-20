namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseQuittanceProductDetail
    {
        public long Id { get; set; }

        public long QuittanceId { get; set; }

        public long ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductCode { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Iva { get; set; } 
    }
}
