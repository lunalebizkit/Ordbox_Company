using Ordbox.Services.Models.Dtos.DtoResponse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestDebitMemo
    {
        public long Id { get; set; }

        [Required]
        public long InvoiceId { get; set; }

        public long InvoiceNumber { get; set; }

        public long CustomerId { get; set; }

        public long UserId { get; set; }

        public long DebitMemoNumber { get; set; }

        public string CustomerName { get; set; }

        public string? CustomerCuit { get; set; }

        public string CustomerAddress { get; set; }

        public string? Observation { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public decimal IvaTotal { get; set; }

        public int Type { get; set; }

        public List<DtoResponseDebitMemoDetails> DebitMemoDetails { get; set; }
    }
    
}
