namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestDetallePrintPDF
    {
        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Iva { get; set; }
    }
}
