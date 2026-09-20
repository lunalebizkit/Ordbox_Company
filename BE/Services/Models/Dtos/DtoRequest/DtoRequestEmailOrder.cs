namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestEmailOrder
    {
        public string Supplier { get; set; }
        public string OrderNumber { get; set; }
        public string Date { get; set; }
        public bool Paid { get; set; }
        public List<DtoRequestEmailOrderDetail> Details { get; set; } = new();
    }
    public class DtoRequestEmailOrderDetail
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
