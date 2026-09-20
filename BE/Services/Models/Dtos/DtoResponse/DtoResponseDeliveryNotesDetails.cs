namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseDeliveryNotesDetail
    {
        public long Id { get; set; }

        public long DeliveryNotesId { get; set; }

        public long ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
