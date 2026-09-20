using Ordbox.Services.Models.Dtos.DtoResponse;


namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestReceipt
    {
        public long Id { get; set; }

        public long SupplierId { get; set; }

        public long UserId { get; set; }

        public long ReceiptNumber { get; set; }

        public string SupplierName { get; set; }

        public string SupplierCuit { get; set; }

        public string SupplierAddress { get; set; }

        public string? Observation { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public decimal IvaTotal { get; set; }


        public decimal ConcNoGravado { get; set; }

        public decimal PercIva { get; set; }


        public decimal PercIngBrutos { get; set; }


        public int Type { get; set; }

        public decimal Iva10 { get; set; }
        public decimal Iva21 { get; set; }
        public decimal Iva27 { get; set; }
        public List<DtoResponseReceiptDetail> ReceiptDetails { get; set; }
        public string? Status { get; set; }

    }
}
