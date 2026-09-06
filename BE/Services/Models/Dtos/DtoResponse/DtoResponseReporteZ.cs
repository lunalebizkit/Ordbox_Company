using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{

    public class DtoResponseReporteZ
    {
        [JsonProperty("CerrarJornadaFiscal")]
        public DtoResponseReporteZBody Body { get; set; }
    }


    public class DtoResponseReporteZBody : BaseEstado
    {

    }
}
