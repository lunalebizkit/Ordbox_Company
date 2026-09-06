using Newtonsoft.Json;


namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    public class CargarDatosCliente
    {
        [JsonProperty("CargarDatosCliente")]
        public object CargarDatosClienteBody { get; set; }
    }

    public class CargarDatosClienteBody
    {
        [JsonProperty("RazonSocial")]
        public string? RazonSocial { get; set; }

        [JsonProperty("NumeroDocumento")]
        public string? NumeroDocumento { get; set; }

        [JsonProperty("ResponsabilidadIVA")]
        public string? ResponsabilidadIVA { get; set; }

        [JsonProperty("TipoDocumento")]
        public string? TipoDocumento { get; set; }

        [JsonProperty("Domicilio")]
        public string? Domicilio { get; set; }
    }
}