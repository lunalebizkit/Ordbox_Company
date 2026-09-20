namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseInvoiceReportTotals
    {
        public DateTime? InvoiceDate { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalSubTotal { get; set; }

        public List<DtoResponseInviocesReport> InvoicesReports { get; set; }
    }
}
