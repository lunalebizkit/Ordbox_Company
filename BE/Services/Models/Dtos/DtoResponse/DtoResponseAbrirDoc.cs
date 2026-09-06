using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{

    public class DtoResponseAbrirDoc
    {
        [JsonProperty("AbrirDocumento")]
        public DtoResponseAbrirDocBody Body { get; set; }
    }
    public  class DtoResponseAbrirDocBody : BaseEstado
    {
        [JsonProperty("NumeroComprobante")]
        public string? NumeroComprobante { get; set; }

        [JsonProperty("IndiceAuditoria")]
        public int IndiceAuditoria { get; set; }

    }
}
