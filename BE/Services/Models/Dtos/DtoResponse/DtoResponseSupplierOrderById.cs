

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseSupplierOrderById
    {
        public long Id { get; set; }

        public long SupplierOrderNumber { get; set; }

        public string SupplierName { get; set; }
        public long SupplierId { get; set; }

        public List<string> SupplierEmail { get; set; }

        public bool IsPaid { get; set; }

        public long StatusId { get; set; }

        public DateTime? DateTime { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public string? Observation { get; set; }

        public List<DtoResponseOrderByIdDetail> OrderDetail { get; set; }


    }
    public class DtoResponseOrderByIdDetail
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public int OrderedQuantity { get; set; }
        public int RecievedQuantity { get; set; }
        public long StatusId { get; set; }

    }
}
