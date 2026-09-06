using Newtonsoft.Json;

namespace Ordbox.SDK.Error
{
    public class OperationExceptions : Exception
    {
        public OperationExceptions(string code, string message, Exception ex = null) : base($"{code} - {message}", ex)
        {
            Info = new OperationExceptionData
            {
                Code = code,
                Descripcion = message
            };
        }

        public OperationExceptionData Info { get; set; }
    }

    public class OperationExceptionData
    {
        [JsonProperty("codigo")]
        public string Code { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

    }
}
