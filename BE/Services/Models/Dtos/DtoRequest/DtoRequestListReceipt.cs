namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestListReceipt
    {
        public long Id { get; set; }

        public long SupplierId { get; set; }

        public string? CreatedBy { get; set; }

        public long ReceiptNumber { get; set; }

        public string SupplierName { get; set; }

        public string SupplierCuit { get; set; }

        public string SupplierAddress { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public int Type { get; set; }

    }
}
