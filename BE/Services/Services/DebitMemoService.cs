using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.ImpresoraFiscal;
using Ordbox.Services.ImpresoraFiscal.Printer250F;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Ordbox.Services.Services
{
    public class DebitMemoService : BaseService
    {
        private readonly IPrinter _printer;
        private readonly PrinterStatus _config;
        public DebitMemoService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration, IPrinter printer, PrinterStatus config) :
            base(logger, context, maper, configuration)
        {
            _config = config;
            _printer = printer;
        }
        public async Task<OperationResponse<DtoRequestDebitMemo>> GetById(long id)
        {
            try
            {
                var debitMemo = await _contextSql
                                    .DebitMemos
                                    .Include(x => x.DebitMemoDetails)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id)
                                    .ConfigureAwait(false);

                if (debitMemo == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoRequestDebitMemo>(new OperationExceptions("000", $"Nota de debito no encontrada Id: {id}"));
                }

                var result = _mapper.Map<DtoRequestDebitMemo>(debitMemo);


                return new OperationResponse<DtoRequestDebitMemo>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }
        public async Task<OperationResponse<IdResponse<long>>> Add(DtoRequestDebitMemo model, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestDebitMemo model, CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            DebitMemo debitMemoModel = null;

            try
            {
                if (model.Id == 0)
                {
                    debitMemoModel = _mapper.Map<DebitMemo>(model);
                    debitMemoModel.InvoiceId = debitMemoModel.InvoiceId == 0 ? null : debitMemoModel.InvoiceId;

                    foreach (DebitMemoDetails debitMemo in debitMemoModel.DebitMemoDetails) { if (debitMemo.ProductId <= 0) { debitMemo.ProductId = -1; } }

                    var regex = new Regex(@"^-?[0-9][0-9,\.]+$");

                    //Verifico que el DNI O CUIT no tenga letras
                    if (!regex.IsMatch(model.CustomerCuit))
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, El CUIT/DNI tiene que ser numerico"));
                    }

                    //Verifico que el DNI tenga mayor a 7 caracteres y menor a 9
                    if (model.Type == 2 && model.CustomerCuit.Length < 7 || model.CustomerCuit.Length > 9 && model.CustomerCuit.Length != 11)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, verifique DNI"));
                    }

                    //Verifico que el CUIT O DNI no se pasen de los parametros
                    if (model.CustomerCuit.Length > 11 || model.CustomerCuit.Length < 7)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, verifique cantidad de digitos"));
                    }

                    //Verfico que la factura A no pueda realizarse al colocar un DNI
                    if (model.Type == 1 && model.CustomerCuit.Length != 11)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, no puede cargar un DNI con Factura tipo A"));
                    }

                    //Verfico que la factura C no pueda realizarse al colocar un DNI
                    if (model.Type == 3 && model.CustomerCuit.Length != 11)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, no puede cargar un DNI con Factura tipo C"));
                    }
                    if (_config.Status) 
                    {
                        var error = await PrintDebitMemo(model, ct);

                        if (error == "ErrorCliente")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, compruebe el CUIT/DNI"));
                        }

                        if (error == "ErrorAbrir")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al abrir documento , intente nuevamente"));
                        }

                        if (error == "ErrorImprimir")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al imprimir item, intente con un cierre Z"));
                        }

                        if (error == "ErrorCerrar")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cerrar documento, intente con un cierre Z"));
                        }

                        debitMemoModel.DebitMemoNumber = long.Parse(error);
                    }
                    await _contextSql.DebitMemos.AddAsync(debitMemoModel, ct).ConfigureAwait(false);

                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                transaction.Commit();
                return Ok(new IdResponse<long>(debitMemoModel.Id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestDebitMemo model, CancellationToken ct = default)
        {
            try
            {
                if (model.Id == 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "La nota de debito no tiene ID"));
                }

                return await AddOrUpdate(model, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<DtoPagination<DtoRequestDebitMemo>>> List(RequestPaginatedData<SpecificFilter> request)
        {
            try
            {
                var query = _contextSql
                                    .DebitMemos
                                    .AsNoTracking()
                                    .Where(p => (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.CustomerCuit.ToLower().Contains(request.Filter.Cuit) : true)
                                     && ((request.Filter.Number.HasValue && request.Filter.Number != 0) ? p.Id == request.Filter.Number : true) &&
                                     ((!request.Filter.Date.Contains("") || request.Filter.Date != null) ? p.DateTime.Date.ToString().Contains(request.Filter.Date) : true)
                                        &&
                                     (!string.IsNullOrEmpty(request.Filter.CustomerName) ? p.CustomerName.ToLower().Contains(request.Filter.CustomerName) : true)
                                     );

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderByDescending(p => p.DateTime)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                var result = _mapper.Map<List<DtoRequestDebitMemo>>(list);


                return new OperationResponse<DtoPagination<DtoRequestDebitMemo>>(new DtoPagination<DtoRequestDebitMemo>
                {
                    Data = result,
                    PageSize = request.PageSize,
                    TotalCount = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<string> PrintDebitMemo(DtoRequestDebitMemo model, CancellationToken ct = default)
        {

            //MANEJO DE ERRORES
            var cargarCliente = await _printer.CargarDatosCliente(model.CustomerName, model.CustomerCuit, model.CustomerAddress, (ETypeReceipt)model.Type).ConfigureAwait(false);

            if (cargarCliente == null)
            {
                await _printer.CerrarJornadaFiscal();
                return "ErrorCliente";
            }

            var openDoc = await _printer.OpenND((ETypeReceipt)model.Type, model.CustomerName, eTypeDocumentClient.Cuil, model.CustomerAddress).ConfigureAwait(false);

            if (openDoc == null)
            {
                await _printer.CloseFactura(1, "").ConfigureAwait(false);
                return "ErrorAbrir";
            }
            //TODO por cada item mandar a imprimir
            foreach (var item in model.DebitMemoDetails)
            {
                var imprimir = await _printer.PrintItem(item.ProductName, item.Quantity, item.Price, item.Iva, item.ProductCode.ToString()).ConfigureAwait(false);

                if (imprimir == null)
                {
                    await _printer.CloseFactura(1, "").ConfigureAwait(false);
                    return "ErrorImprimir";
                }
            }

            var closeFactura = await _printer.CloseFactura(1, "").ConfigureAwait(false);

            if (closeFactura == null)
            {
                await _printer.CerrarJornadaFiscal();
                return "ErrorCerrar";
            }

            return closeFactura;

        }
    }
}
