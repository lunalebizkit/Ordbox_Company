using Newtonsoft.Json;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    public class ConfigurarZona
    {
        [JsonProperty("ConfigurarZona")]
        public object? ConfigurarZonaBody { get; set; }
    }

    public class ConfigurarZonaBody
    {
        [JsonProperty("NumeroLinea")]
        public int? NumeroLinea { get; set; }

        [JsonProperty("Atributos")]
        public string[] Atributos { get; set; } = {"Centrado", "DobleAncho"};

        [JsonProperty("Descripcion")]
        public string? Descripcion { get; set; }

        [JsonProperty("Estacion")]
        public string Estacion { get; set; } = "EstacionPorDefecto";  
        
        [JsonProperty("IdentificadorZona")]
        public string IdentificadorZona { get; set; } = "ZonaDomicilioEmisor";
    }
}
