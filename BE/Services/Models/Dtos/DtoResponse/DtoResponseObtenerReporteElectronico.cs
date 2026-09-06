using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseObtenerReporteElectronico
    {
        [JsonProperty("ObtenerPrimerBloqueReporteElectronico")]
        public DtoResponseObtenerReporteElectronicoBody BloqueElectronico { get; set; }
    }

    public class DtoResponseObtenerReporteElectronicoBody: BaseEstado
    {
        [JsonProperty("Registro")]
        public string Registro { get; set; }

        [JsonProperty("Informacion")]
        public string Informacion { get; set; }
    }
}
