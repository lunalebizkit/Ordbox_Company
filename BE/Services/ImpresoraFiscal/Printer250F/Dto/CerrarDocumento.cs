using Newtonsoft.Json;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{

    public class CerrarDocumento
    {
        [JsonProperty("CerrarDocumento")]
        public CerrarDocumentoBody CerrarDocumentoBody { get; set; }
    }
    public class CerrarDocumentoBody
    {
        [JsonProperty("Copias")]
        public int Copias { get; set; }

        [JsonProperty("DireccionEmail")]
        public string? DireccionEmail { get; set; }

    }
}