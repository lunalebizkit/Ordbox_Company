namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseProductReportTotal
    {
        public decimal? Total { get; set; }
        public DateTimeOffset? Date { get; set; }
        public List<DtoResponseProductReport> Products { get; set; }
    }
    public class DtoResponseProductReport
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Purchase_Price { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
