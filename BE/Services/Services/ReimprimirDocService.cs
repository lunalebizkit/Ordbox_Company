using AutoMapper;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.ImpresoraFiscal.Printer250F;
using Ordbox.Services.ImpresoraFiscal;
using Ordbox.Domain;
using Ordbox.Domain.Enum;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class ReimprimirDocService : BaseService
    {
        private readonly IPrinter _printer;
        private readonly PrinterStatus _config;

        public ReimprimirDocService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration, IPrinter printer,
            PrinterStatus config) :
            base(logger, context, maper, configuration)
        {
            _config = config;
            _printer = printer;
        }

        public async Task<OperationResponse<bool>> ReimprmirDoc(ETypeReceipt tipoDocumento, string numeroComprobante)
        {
            try
            {
                int tipoDoc=0;

                if (tipoDocumento == ETypeReceipt.B || tipoDocumento == ETypeReceipt.EXENTO)
                {
                    tipoDoc = (int)ETypePrint.BImpresion;
                }

                if (tipoDocumento == ETypeReceipt.C)
                {
                    tipoDoc = (int)ETypePrint.CImpresion;
                }

                if (numeroComprobante == "0")
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>(new OperationExceptions("000",
                        "El numero de comprobante no puede ser 0"));
                }

                if (_config.Status == false)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>(new OperationExceptions("000",
                        "La impresora esta desactivada, reactive para realizar el Reimpresion"));
                }

                var reimpirmirDoc = await _printer.ReimprimirDocumento(tipoDoc, numeroComprobante);

                if (reimpirmirDoc == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>(new OperationExceptions("000",
                        "No se pudo realizar la Reimpresion , verifique conexion a la impresora"));
                }

                return new OperationResponse<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
    }
}
