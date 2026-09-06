using Ordbox.Services.Models.Dtos.DtoResponse;
using Newtonsoft.Json;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    public class DtoResponseObtenerSiguienteBloqueReporteElectronico
    {
        [JsonProperty("ObtenerSiguienteBloqueReporteElectronico")]
        public DtoResponseObtenerReporteElectronicoBody BloqueElectronico {  get; set; }
    }
}
