using Newtonsoft.Json;


namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    public class AbrirDocumento
    {
        [JsonProperty("AbrirDocumento")]
        public AbrirDocumentoBody AbrirDocumentoBody { get; set; }
    }
    public class AbrirDocumentoBody
    {
        [JsonProperty("CodigoComprobante")]
        public string CodigoComprobante { get; set; }
    }
}
