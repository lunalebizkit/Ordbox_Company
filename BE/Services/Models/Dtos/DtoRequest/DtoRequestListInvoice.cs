namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestListInvoice
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public string createdBy { get; set; }

        public long InvoiceNumber { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerCuit { get; set; }

        public string? CustomerAddress { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public int Type { get; set; }
        public string? Status { get; set; }
        public string? CAE { get; set; }

        public DateTime? CAEExpirationDate { get; set; }

        public bool? IntegrationSuccess { get; set; }

        public byte Version { get; set; }

    }
}
