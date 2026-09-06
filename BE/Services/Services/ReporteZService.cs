using AutoMapper;
using Ordbox.Domain;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.ImpresoraFiscal;
using Ordbox.Services.ImpresoraFiscal.Printer250F;
using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Ordbox.Services.Services
{
    public  class ReporteZService : BaseService
    {
        private readonly IPrinter _printer;
        private readonly PrinterStatus _config;
        public ReporteZService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration, IPrinter printer, PrinterStatus config) :
            base(logger, context, maper, configuration)
        {
            _config = config;
            _printer = printer;
        }
        public async Task<OperationResponse<bool>> ReporteZ()
        {
            try
            {
                if (_config.Status == false)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>(new OperationExceptions("000", "La impresora esta desactivada, reactive para realizar el Reporte Z"));
                }
                

                var cerrarJornada = await _printer.CerrarJornadaFiscal();

                if(cerrarJornada == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>(new OperationExceptions("000", "No se pudo realizar reporte Z , verifique conexion a la impresora"));
                }

                return new OperationResponse<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<bool>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION)));
            }
        }
        public async Task<OperationResponse<DtoResponseConsultarVersion>> PrintSettings()
        {
            try
            {
                if (_config.Status == false)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseConsultarVersion>(new OperationExceptions("000", "La impresora esta desactivada"));
                }
                

                var estadoImpresora = await _printer.ConsultarVersionImpresora(new ConsultarVersion() { });

                if(estadoImpresora == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseConsultarVersion>(new OperationExceptions("000", "No se acceder a la impresora, verifique conexion a la impresora"));
                }

                return new OperationResponse<DtoResponseConsultarVersion>(estadoImpresora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<DtoResponseConsultarVersion>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION)));
            }
        }
        
        public async Task<OperationResponse<byte[]>> DownloadPrintReport(ObtenerPrimerBloqueReporteElectronicoBody bloqueReporteElectronicoBody)
        {
            try
            {
                if (_config.Status == false)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<byte[]>(new OperationExceptions("000", "La impresora esta desactivada"));
                }

                var allBytes = new List<byte>();

                var firstReport = await _printer.DownloadPrintReport(new ObtenerPrimerBloqueReporteElectronico() {
                    ObtenerPrimerBloqueReporteElectronicoBody = bloqueReporteElectronicoBody });

                if (firstReport == null || string.IsNullOrEmpty(firstReport?.BloqueElectronico?.Registro))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<byte[]>(new OperationExceptions("000", "No se acceder a la impresora, verifique conexion a la impresora"));
                }

                if (firstReport.BloqueElectronico?.Registro == "BloqueInformacion")
                {
                    byte[] primerBloque = ProcesarBloque(firstReport.BloqueElectronico);
                    allBytes.AddRange(primerBloque);

                    bool fin = false;
                    while (!fin)
                    {
                        DtoObtenerSiguienteBloqueReporteElectronico siguienteComando =  new () ;
                        var respuesta = await _printer.DownloadNextBloquePrintReport(siguienteComando);
                        byte[] bloque = ProcesarBloque(respuesta.BloqueElectronico, out fin);
                        allBytes.AddRange(bloque);
                    }

                }            

                return new OperationResponse<byte[]>(allBytes.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<byte[]>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION)));
            }
        }

        #region Private

        private byte[] ProcesarBloque(DtoResponseObtenerReporteElectronicoBody dtoResponseObtener, out bool esFinal)
        {

            if ((dtoResponseObtener.Registro == "BloqueInformacion") ||
                (dtoResponseObtener.Registro == "BloqueFinal"))
            {
                string registro = dtoResponseObtener.Registro;
                string informacion = dtoResponseObtener.Informacion;

                // Decodificar ASCII85
                byte [] decoded = Decode(informacion);

                esFinal = registro == "BloqueFinal";
                return decoded;
            }

            esFinal = true;
            return Array.Empty<byte>();
        }

        private byte[] ProcesarBloque(DtoResponseObtenerReporteElectronicoBody dtoResponseObtenerReporte)
        {
            return ProcesarBloque(dtoResponseObtenerReporte, out _);
        }

        public static byte[] Decode(string input, Encoding? outputEncoding = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<byte>();

            // Usar UTF-8 por defecto, pero configurable
            outputEncoding ??= Encoding.UTF8;

            // Remover delimitadores si existen
            if (input.StartsWith("<~")) input = input.Substring(2);
            if (input.EndsWith("~>")) input = input.Substring(0, input.Length - 2);

            List<byte> result = new List<byte>();
            int count = 0;
            uint tuple = 0;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c))
                    continue;

                if (c == 'z')
                {
                    if (count != 0)
                        throw new FormatException("Carácter 'z' inválido en bloque parcial.");

                    result.AddRange(new byte[] { 0, 0, 0, 0 });
                    continue;
                }

                if (c < '!' || c > 'u')
                    throw new FormatException($"Carácter inválido en ASCII85: {c}");

                tuple = tuple * 85 + (uint)(c - '!');
                count++;

                if (count == 5)
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        result.Add((byte)((tuple >> (8 * i)) & 0xFF));
                    }
                    count = 0;
                    tuple = 0;
                }
            }

            // Manejo de bloque incompleto
            if (count > 0)
            {
                for (int i = count; i < 5; i++)
                    tuple = tuple * 85 + 84; // padding

                for (int i = 3; i >= 0; i--)
                {
                    result.Add((byte)((tuple >> (8 * i)) & 0xFF));
                }

                // Eliminar bytes extra de padding
                int extraBytes = 5 - count;
                if (extraBytes > 0 && extraBytes <= result.Count)
                    result.RemoveRange(result.Count - extraBytes, extraBytes);
            }

            return result.ToArray();
        }


        #endregion
    }
}
