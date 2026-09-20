namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestCabeceraPrintPDF
    {
        public string Nombre { get; set; }

        public long Number { get; set; }
        public long RelatedNumber { get; set; }

        public string Direccion { get; set; }

        public string CustomerCuit { get; set; }

        public string? Observacion { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime CAEExpirationDate { get; set; }

        public string DocumentType { get; set; }

        public int Type { get; set; }

        public int ArcaType { get; set; }

        public decimal Total { get; set; }

        public decimal IvaTotal { get; set; }

        public string CAE { get; set; }

        public decimal? Iva21 { get; set; }

        public decimal? Iva27 { get; set; }

        public decimal? Iva10 { get; set; }

        public List<DtoRequestDetallePrintPDF> Details { get; set; }
    }
}
