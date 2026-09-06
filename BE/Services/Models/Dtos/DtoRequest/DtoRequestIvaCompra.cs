using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestIvaPeriodCompra
    {
        public decimal? PeriodTotal { get; set; }
        public List<DtoRequestIvaCompra> Receipts { get; set; }
    }
    public class DtoRequestIvaCompra
    {
        public long Id { get; set; }
        public long ReceiptNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCuit { get; set; }
        public string SupplierAddress { get; set; }
        public DateTime DateTime { get; set; }
        public decimal? Total { get; set; }
        public int Type { get; set; }
        public decimal ConcNoGravado { get; set; }
        public decimal PercIva { get; set; }
        public decimal PercIngBrutos { get; set; }
        public List<DtoRequestIvaCompraDetails> ReceiptDetails { get; set; }
    }
    public class DtoRequestIvaCompraDetails
    {
        public decimal Iva { get; set; }
        public decimal Iva21 { get; set; }
        public decimal Iva27 { get; set; }
        public decimal Iva10 { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}
