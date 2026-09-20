

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestIvaPeriodVenta
    {
        public decimal PeriodTotal { get; set; }
        public List<DtoRequestIvaVenta> Invoices { get; set; }
    }
    public class DtoRequestIvaVenta
    {
        public long Id { get; set; }
        public long InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCuit { get; set; }
        public DateTime DateTime { get; set; }
        public int Type { get; set; }
        public decimal Total { get; set; }
        public List<DtoRequestIvaVentaDetails> InvoiceDetails { get; set; }
    }
    public class DtoRequestIvaVentaDetails
    { 
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Iva { get; set; }
        public decimal Iva21 { get; set; }
        public decimal Iva27 { get; set; }
        public decimal Iva10 { get; set; }

    }
}
