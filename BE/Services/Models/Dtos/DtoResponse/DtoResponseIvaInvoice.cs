


namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseIvaInvoice
    {
        public decimal PeriodTotal { get; set; }
        public List<DtoResponseIvaInvoices> DtoResponseIvaInvoices { get; set; }
    }
  
    public class DtoResponseIvaInvoices
    {
        public long Id { get; set; }
        public long InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCuit { get; set; }
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public int Type { get; set; }
        public decimal Iva21 { get; set; }
        public decimal Iva27 { get; set; }
        public decimal Iva10 { get; set; }
        public decimal ImporteNetoIva21 { get; set; }
        public decimal ImporteNetoIva27 { get; set; }
        public decimal ImporteNetoIva10 { get; set; }
        public decimal ImporteNeto { get; set; }

        public decimal IvaTotal { get; set; }
    }
}
