using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseIvaReceipt
    {

        public decimal PeriodTotal { get; set; }
        public List<DtoResponseIvaReceipts> DtoResponseIvaReceipts { get; set; }


    }
    public class DtoResponseIvaReceipts
    {
        public long Id { get; set; }
        public long ReceiptNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCuit { get; set; }
        public string SupplierAddress { get; set; }
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public int Type { get; set; }       
        public decimal Iva21 { get; set; }
        public decimal Iva27 { get; set; }
        public decimal Iva10 { get; set; }
        public decimal ImporteNetoIva21 { get; set; }
        public decimal ImporteNetoIva27 { get; set; }
        public decimal ImporteNetoIva10 { get; set; }
        public decimal ImporteNeto { get; set; }
        public decimal ConcNoGravado { get; set; }
        public decimal PercIva { get; set; }
        public decimal PercIngBrutos { get; set; }
        public decimal IvaTotal { get; set; }
    }
}

