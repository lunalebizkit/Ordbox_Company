using System;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestDeliveryNotes
    {
        public long Id { get; set; }

        public long DeliveryNotesNumber { get; set; }

        public DateTime DateTime { get; set; }

        public long SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public string? SupplierCuit { get; set; }

        public string? SupplierAddress { get; set; }

        public long StatusId { get; set; }

        public string? Cancelled { get; set; }

        public Boolean Paid { get; set; }

        public string? Observation { get; set; }       

        public decimal ImportTotal { get; set; }

        public List<DtoResponseDeliveryNotesDetail> DeliveryNotesDetails { get; set; }

    }
}
