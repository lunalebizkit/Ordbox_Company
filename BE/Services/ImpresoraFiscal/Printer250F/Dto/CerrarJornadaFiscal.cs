using Newtonsoft.Json;


namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{

    public class CerrarJornadaFiscal
    {
        [JsonProperty("CerrarJornadaFiscal")]
        public  object CerrarJornadaFiscalBody { get; set; }
    }
    public class CerrarJornadaFiscalBody
    {
        [JsonProperty("Reporte")]
        public string Reporte { get; set; }

    }
}

