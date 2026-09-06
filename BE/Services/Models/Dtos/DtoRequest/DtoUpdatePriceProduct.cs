namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoUpdatePriceProduct
    {

        public int IdPrice { get; set; }

        public decimal Value { get; set; }

        public string? Product { get; set; }

        public long? Brand { get; set; }

        public long? Category { get; set; }

        public List<long>? Supplier { get; set; }

    }
}
