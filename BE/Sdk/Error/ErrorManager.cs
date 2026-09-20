
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace Ordbox.SDK.Error
{
    public class ErrorManager
    {

        /// <summary>
        /// Logger para el manejo de los mensajes de error
        /// </summary>
        public readonly ILogger _logger;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public ErrorManager(ILogger<ErrorManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Log un error y devuelve la exception generada
        /// </summary>
        /// <returns></returns>
        public OperationExceptions LogError(string code, object data = null, Exception ex = null)
        {
            var opEx = new OperationExceptions(code, ErrorsMessages.GetMessage(code), ex);
            _logger.LogError(ex, $"{code} - {ErrorsMessages.GetMessage(code)}: Data {(data != null ? JsonConvert.SerializeObject(data) : String.Empty)}");
            return opEx;
        }

        public void LogWarning(string code, object data = null)
        {
            _logger.LogWarning($"{code} - {ErrorsMessages.GetMessage(code)}: Data {(data != null ? JsonConvert.SerializeObject(data) : String.Empty)}");
        }

        public void LogInfo(string code, object data = null)
        {
            _logger.LogInformation($"{code} - {ErrorsMessages.GetMessage(code)}: Data {(data != null ? JsonConvert.SerializeObject(data) : String.Empty)}");
        }

        public void LogRequestAndResponseInfo(string data)
        {
            _logger.LogInformation(data);
        }
    }
}
