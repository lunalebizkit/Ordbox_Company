using Newtonsoft.Json;


namespace Ordbox.Services.ImpresoraFiscal.Printer250F.Dto
{

        public class BaseEstado
        {
            [JsonProperty("Estado")]
            public Estadobody? Estado { get; set; }

            [JsonProperty("Secuencia")]
            public int Secuencia { get; set; }
        }

        public class Estadobody 
        {
         
            [JsonProperty("Impresora")]
            public string[] Impresora { get; set; }

            [JsonProperty("Fiscal")]
            public string[] Fiscal { get; set; }
        }
}
