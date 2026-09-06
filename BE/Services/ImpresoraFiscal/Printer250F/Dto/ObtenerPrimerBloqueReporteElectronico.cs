using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    /// <summary>
    /// 
    /// </summary>

    public class ObtenerPrimerBloqueReporteElectronico
    {
        [JsonProperty("ObtenerPrimerBloqueReporteElectronico")]
        public ObtenerPrimerBloqueReporteElectronicoBody ObtenerPrimerBloqueReporteElectronicoBody { get; set; }
    }

    public class ObtenerPrimerBloqueReporteElectronicoBody
    {
        [JsonPropertyName("FechaInicial")]
        public string? FechaInicial { get; set; }

        [JsonPropertyName("FechaFinal")]
        public string? FechaFinal { get; set;}

        [JsonPropertyName("TipoReporte")]
        public string? TipoReporte { get; set; } = "ReporteAFIPCompleto";
    }
}
