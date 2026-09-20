

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestSupplierOrder
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DateTime { get; set; }
        public long SupplierOrderNumber { get; set; }
        public long StatusId { get; set; }
        public List<string> SupplierEmail { get; set; }
        public string? Observation { get; set; }
        public List<DtoRequestOrderDetail> OrderDetail { get; set; }


    }
    public class DtoRequestOrderDetail
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int OrderedQuantity { get; set; }
        public int RecievedQuantity { get; set; }
        public long StatusId { get; set; }


    }
}

