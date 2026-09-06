namespace Ordbox.Domain.Model
{
    public class InvoiceSPReport
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
    }
}
