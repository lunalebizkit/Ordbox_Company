using Newtonsoft.Json;

namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{
    public class ImprimirItem
    {
        [JsonProperty("ImprimirItem")]
        public object ImprimirItemBody { get; set; }
    }
    public class ImprimirItemBody
    {
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("Cantidad")]
        public double Cantidad { get; set; }

        [JsonProperty("PrecioUnitario")]
        public decimal PrecioUnitario { get; set; }

        [JsonProperty("CondicionIVA")]
        public string CondicionIVA { get; set; } = "Gravado";

        [JsonProperty("AlicuotaIVA")]
        public decimal AlicuotaIVA { get; set; }

        [JsonProperty("OperacionMonto")]
        public string OperacionMonto { get; set; } = "ModoSumaMonto";

        [JsonProperty("TipoImpuestoInterno")]
        public string TipoImpuestoInterno { get; set; } = "IIFijoMonto";

        [JsonProperty("MagnitudImpuestoInterno")]
        public string MagnitudImpuestoInterno { get; set; } = "0.00";

        [JsonProperty("ModoDisplay")]
        public string ModoDisplay { get; set; } = "DisplayNo";      
        
        [JsonProperty("ModoBaseTotal")]
        public string ModoBaseTotal { get; set; } = "ModoPrecioTotal";

        [JsonProperty("UnidadReferencia")]
        public string UnidadReferencia { get; set; } = "1";

        [JsonProperty("CodigoProducto")]
        public string CodigoProducto { get; set; } = "";

        [JsonProperty("CodigoInterno")]
        public string CodigoInterno { get; set; } = "C1130";

        [JsonProperty("UnidadMedida")]
        public string UnidadMedida { get; set; } = "Unidad";
    }
}
