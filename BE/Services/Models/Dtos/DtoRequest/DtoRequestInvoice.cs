using System;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestInvoice
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public long UserId { get; set; }

        public long InvoiceNumber { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCuit { get; set; }

        public string CustomerAddress { get; set; }

        public string? Observation { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public decimal IvaTotal { get; set; }

        public decimal? Iva21 { get; set; }

        public decimal? Iva27 { get; set; }

        public decimal? Iva10 { get; set; }

        public int Type { get; set; }

        public string? Status { get; set; }

        public string? CAE { get; set; }

        public DateTime? CAEExpirationDate { get; set; }

        public bool IntegrationSuccess { get; set; }

        public long? InvoiceNumberArca { get; set; }

        public byte Version { get; set; }

        public List<DtoResponseInvoiceDetail> InvoiceDetails { get; set; }

    }
}
