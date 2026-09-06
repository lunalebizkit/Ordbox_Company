namespace Ordbox.Services.Common
{
    public class RequestPaginatedData<T>
    {
        public required T Filter { get; set; }
        public int Page { get; set; }

        int? _pageSize = default;
        public int PageSize { get => _pageSize ?? 1000; set => _pageSize = value; }
    }
    public class UserFilter
    {
        public required string Query { get; set; }
        public required long[] Rol { get; set; }
    }
    public class ProductFilter
    {
        public string? Product { get; set; }
        public string? BarCode { get; set; }
        public string? Code { get; set; }
        public long? Brand { get; set; }
        public long? Category { get; set; }
        public long? Status { get; set; }
        public DateTime? Date { get; set; }
        public required List<long> Supplier { get; set; }
    }
    public class SpecificFilter
    {
        public string? Supplier { get; set; }
        public string? Category { get; set; }
        public int? StatusId { get; set; }
        public long? Number { get; set; }
        public string? Cuit { get; set; }
        public string? Date { get; set; }
        public string? CustomerName { get; set; }

    }
    public class PeriodFilter
    {
        public DateTime Date { get; set; }
    } 
    public class StoredProcedureFilter
    {
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public int? CategoryId { get; set; }
    }

}
