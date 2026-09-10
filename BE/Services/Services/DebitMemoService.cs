using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.Domain.Model.Extensions;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
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

    }
}
