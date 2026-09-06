using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public partial class DtoResponseReimprimirDoc
    {

        [JsonProperty("CopiarComprobante")]
        public DtoResponseReimprimirDoc? Body { get; set; }
    }

    public partial class DtoResponseReimprimirDoc : BaseEstado
    {
        [JsonProperty("Zeta")]
        public string? Zeta { get; set; }

        [JsonProperty("Indice")]
        public int Indice { get; set; }
    }
}
