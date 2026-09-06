using Newtonsoft.Json;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    /// <summary>
    /// Retorna version de impresora fiscal
    /// </summary>
    public class ConsultarVersion
    {
        /// <summary>
        /// Propiedad 
        /// </summary>
        [JsonProperty("ConsultarVersion")]
        public object ConsultarVersionBody { get; set; } = new { };
    }
}
