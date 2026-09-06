namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseCreditMemoDetails
    {
        public long Id { get; set; }

        public long CreditId { get; set; }

        public long ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductCode { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Iva { get; set; }
       
    }
}
