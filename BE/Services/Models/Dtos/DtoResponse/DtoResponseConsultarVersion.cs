using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Newtonsoft.Json;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    /// <summary>
    /// Retorna la informacion de version de impresora fiscal 
    /// </summary>
    public class DtoResponseConsultarVersion
    {
        [JsonProperty("ConsultarVersion")]
        public DtoResponseConsultarVersionBody ConsultarVersionBody { get; set; }
    }

    /// <summary>
    /// Retorna la informacion del cuerpo version de impresora fiscal 
    /// </summary>
    public class DtoResponseConsultarVersionBody: BaseEstado 
    {
        /// <summary>
        /// version
        /// </summary>
        [JsonProperty("Version")]
        public string? Version { get; set; }
        /// <summary>
        /// marca
        /// </summary>
        [JsonProperty("Marca")]
        public string? Marca { get; set; }
        /// <summary>
        /// nombre de producto
        /// </summary>
        [JsonProperty("NombreProducto")]
        public string? NombreProducto { get; set; }
        /// <summary>
        /// version de motor
        /// </summary>
        [JsonProperty("VersionMotor")]
        public string? VersionMotor { get; set; }
        /// <summary>
        /// fecha dispositivo
        /// </summary>
        [JsonProperty("FechaFirmware")]
        public string? FechaFirmware { get; set; }
        /// <summary>
        /// version de protocolo
        /// </summary>
        [JsonProperty("VersionProtocolo")]
        public string? VersionProtocolo { get; set; }
    }
}
