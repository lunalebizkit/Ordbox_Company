namespace Ordbox.Domain.Model
{
    public class InvoiceSPReportTotal
    {
        public long? Id { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalSubTotal { get; set; }

    }
}
