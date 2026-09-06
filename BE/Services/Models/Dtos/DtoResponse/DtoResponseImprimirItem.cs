using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{

    public class DtoResponseImprimirItem
    {
        [JsonProperty("ImprimirItem")]
        public DtoResponseImprimirItemBody Body { get; set; }
    }

    public class DtoResponseImprimirItemBody : BaseEstado
    {
        [JsonProperty("IndiceAuditoria")]
        public int IndiceAuditoria { get; set; }

    }
}
