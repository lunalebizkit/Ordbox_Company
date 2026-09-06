using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;


namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseCerrarDoc 
    {

        [JsonProperty("CerrarDocumento")]
        public DtoResponseCerrarDocBody Body { get; set; }
    }

    public class DtoResponseCerrarDocBody : BaseEstado
    {
        [JsonProperty("NumeroComprobante")]
        public string NumeroComprobante { get; set; }

        [JsonProperty("CantidadDePaginas")]
        public int CantidadDePaginas { get; set; }

        [JsonProperty("IndiceAuditoria")]
        public int IndiceAuditoria { get; set; }
    }
}
