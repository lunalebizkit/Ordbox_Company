

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseSupplierOrder
    {
        public long Id { get; set; }

        public long SupplierOrderNumber { get; set; }

        public string SupplierName { get; set; }

        public bool IsPaid { get; set; }

        public long StatusId { get; set; }

        public string? Observation { get; set; }

        public DateTime? DateTime { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public List<DtoResponseOrderDetail> OrderDetail { get; set; }


    }
    public class DtoResponseOrderDetail
    {
        public long Id { get; set; }
        public string ProductId { get; set; }
        public int OrderedQuantity { get; set; }
        public int RecievedQuantity { get; set; }
        public long StatusId { get; set; }


    }

}
