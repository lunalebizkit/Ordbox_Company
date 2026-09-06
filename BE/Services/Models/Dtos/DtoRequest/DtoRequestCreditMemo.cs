

using Ordbox.Services.Models.Dtos.DtoResponse;
using System.ComponentModel.DataAnnotations;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestCreditMemo
    {
        public long Id { get; set; }

        [Required]
        public long InvoiceId { get; set; }

        public long InvoiceNumber { get; set;  }

        public long CustomerId { get; set; }

        public long UserId { get; set; }

        public long CreditMemoNumber { get; set; }

        public string CustomerName { get; set; }

        public string? CustomerCuit { get; set; }

        public string CustomerAddress { get; set; }

        public string? Observation { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public decimal IvaTotal { get; set; }
        public decimal? Iva21 { get; set; }
        public decimal? Iva27 { get; set; }
        public decimal? Iva10 { get; set; }

        public int Type { get; set; }

        public List<DtoResponseCreditMemoDetails> CreditMemoDetail { get; set; }

    }
}
