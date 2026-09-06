namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    /// <summary>
    /// 
    /// </summary>
    public class DtoRequestQuittance
    {
        public long Id { get; set; }

        public int QuittanceNumber { get; set; }

        public string? CustomerName { get; set; }

        public string? Address { get; set; }

        public string? CustomerCuit { get; set; }

        public DateTime DateTime { get; set; }

        public string? Money { get; set; }

        public string? Concept { get; set; }

        public decimal? Cash { get; set; }

        public decimal? Total { get; set; }

        public List<DtoRequesQuittanceDetails?>? QuittanceDetails { get; set; }
        public List<DtoRequestQuittanceProductDetail>? QuittanceProductDetails { get; set; }

    }
    public class DtoRequesQuittanceDetails
    {

        public long QuittanceId { get; set; }

        public string? CheckNumber { get; set; }

        public decimal? Total { get; set; }

        public int? Quantity { get; set; }

        public string? Bank { get; set; }
    }
}
