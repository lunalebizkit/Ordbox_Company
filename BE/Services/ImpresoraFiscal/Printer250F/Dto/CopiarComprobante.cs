using Ordbox.Domain.Enum;
using Newtonsoft.Json;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    public class CopiarComprobante
    {
        [JsonProperty("CopiarComprobante")]
        public object? CopiarComprobanteBody { get; set; }
    }
    public class CopiarComprobanteBody
    {
        [JsonProperty("CodigoComprobante")]
        public int? CodigoComprobante { get; set; }

        [JsonProperty("NumeroComprobante")]
        public string? NumeroComprobante { get; set; }

        [JsonProperty("Imprimir")]
        public string Imprimir { get; set; } = "Si";
    }
}
