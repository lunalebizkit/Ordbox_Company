using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.Domain.Model.Extensions;
using Ordbox.SDK.Error;
using Ordbox.Services.ARCA.Dto.Response;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Scripts;
using System.Text.RegularExpressions;

namespace Ordbox.Services.Services
{
    public class DebitMemoService : BaseService
    {
        public DebitMemoService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
            base(logger, context, maper, configuration)
        {
        }
        public async Task<OperationResponse<DtoRequestDebitMemo>> GetById(long id, RequestedBy requestedBy)
        {
            try
            {
                var debitMemo = await _contextSql
                                    .DebitMemos
                                    .Include(x => x.DebitMemoDetails)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == requestedBy.CompanyId)
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

        public async Task<OperationResponse<IdResponse<long>>> Add(DtoRequestDebitMemo model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);
        }

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestDebitMemo model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            DebitMemo debitMemoModel = null;

            try
            {
                if (model.Id == 0)
                {
                    debitMemoModel = _mapper.Map<DebitMemo>(model);
                    debitMemoModel.CompanyId = requestedBy.CompanyId;
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

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestDebitMemo model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                if (model.Id == 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "La nota de debito no tiene ID"));
                }

                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoRequestDebitMemo>>> List(RequestPaginatedData<SpecificFilter> request, RequestedBy requestedBy)
        {
            try
            {
                var query = _contextSql
                                    .DebitMemos
                                    .AsNoTracking()
                                    .Where(p => p.CompanyId == requestedBy.CompanyId && (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.CustomerCuit.ToLower().Contains(request.Filter.Cuit) : true)
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

        public async Task<OperationResponse<DtoRequestCabeceraPrintPDF>> GetDocumentById(long id)
        {
            try
            {
                var factura = await _contextSql
                                    .DebitMemos
                                    .Include(x => x.DebitMemoDetails)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id)
                                    .ConfigureAwait(false);
                if (factura == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoRequestCabeceraPrintPDF>(new OperationExceptions("000", $"Factura no encontrada {id}"));
                }

                var result = _mapper.Map<DtoRequestCabeceraPrintPDF>(factura);

                result.Iva10 = 0;
                result.Iva21 = 0;
                result.Iva27 = 0;
                foreach (var item in result.Details)
                {
                    result.Iva10 += ((decimal)item.Iva == (decimal)10.5) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.105m : 0;
                    result.Iva21 += ((decimal)item.Iva == (decimal)21) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.21m : 0;
                    result.Iva27 += ((decimal)item.Iva == (decimal)27) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.27m : 0;
                }


                return new OperationResponse<DtoRequestCabeceraPrintPDF>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestDebitMemo model, DtoResponseARCAInvoice responseARCAInvoice, CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            var debitModel = _mapper.Map<DebitMemo>(model);
            var newProduct = new Product();
            try
            {
                if (debitModel.Id != 0)
                {
                    if (debitModel.CustomerId == 0)
                    {
                        debitModel.CustomerId = GetUserAdminId();
                    }

                    DebitMemo debitMemo = await _contextSql.DebitMemos.FirstAsync(p => p.Id == debitModel.Id).ConfigureAwait(false);

                    debitModel.CAE = string.IsNullOrEmpty(responseARCAInvoice.Cae) ? null : responseARCAInvoice.Cae;
                    debitModel.CAEExpirationDate = responseARCAInvoice.FechaVencimientoCae.HasValue ? responseARCAInvoice.FechaVencimientoCae.Value : null;
                    debitModel.IntegrationSuccess = !string.IsNullOrEmpty(responseARCAInvoice.Cae);
                    debitModel.DebitMemoNumber = responseARCAInvoice.InvoiceNumber;

                    _contextSql.Entry(debitMemo).State = EntityState.Detached;
                    _contextSql.DebitMemos.Update(debitModel);
                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                transaction.Commit();
                return Ok(new IdResponse<long>(debitModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO)));
            }
        }

        #region Private Methods
        private byte GetUserAdminId()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    return connection.Query<byte>(SqlScripts.GetUserAdminId).First();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        #endregion

    }
}
