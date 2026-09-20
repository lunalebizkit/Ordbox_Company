namespace Ordbox.Services.ARCA.Dto.Response
{
    public class DtoResponseARCAInvoice
    {
        public long InvoiceNumber { get; set; }
        public string Resultado { get; set; }
        public string Cae { get; set; }
        public DateTime? FechaVencimientoCae { get; set; }
        public List<string> Observaciones { get; set; } = new();
        public List<string> Errores { get; set; } = new();
    }
}
