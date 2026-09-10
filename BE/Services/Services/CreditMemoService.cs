using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.Domain.Model.Extensions;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Scripts;
using System.Text.RegularExpressions;

namespace Ordbox.Services.Services
{
    public class CreditMemoService : BaseService
    {
        public CreditMemoService(ErrorManager logger, DBContext context, IMapper mapper, IConfiguration configuration) :
          base(logger, context, mapper, configuration)
        {
        }

        //Metodo Get By Id
        public async Task<OperationResponse<DtoRequestCreditMemo>> GetById(long id, RequestedBy requestedBy)
        {
            try
            {
                var creditMemo = await _contextSql
                                    .CreditMemo
                                    .Include(x => x.CreditMemoDetail)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == requestedBy.CompanyId)
                                    .ConfigureAwait(false);

                if (creditMemo == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoRequestCreditMemo>(new OperationExceptions("000", $"Nota de Credito no encontrado ID: {id}"));
                }

                var result = _mapper.Map<DtoRequestCreditMemo>(creditMemo);
                result.Iva10 = 0;
                result.Iva21 = 0;
                result.Iva27 = 0;
                foreach (var item in result.CreditMemoDetail)
                {
                    result.Iva10 += ((decimal)item.Iva == (decimal)10.5) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.105m : 0;
                    result.Iva21 += ((decimal)item.Iva == (decimal)21) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.21m : 0;
                    result.Iva27 += ((decimal)item.Iva == (decimal)27) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.27m : 0;
                }
                return new OperationResponse<DtoRequestCreditMemo>(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> NewMemo(DtoRequestCreditMemo model, RequestedBy requestedBy, CancellationToken ct = default)
        {
                model.Id = 0;
                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);
        }

        public async Task<OperationResponse<DtoPagination<DtoRequestCreditMemo>>> List(RequestPaginatedData<SpecificFilter> request, RequestedBy requestedBy)
        {
            try
            {
                var query = _contextSql
                                    .CreditMemo                                 
                                    .AsNoTracking()
                                    .Where(p => p.CompanyId == requestedBy.CompanyId && (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.CustomerCuit.ToLower().Contains(request.Filter.Cuit) : true)
                                     && ((request.Filter.Number.HasValue && request.Filter.Number != 0) ? p.InvoiceNumber == request.Filter.Number : true) &&
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

                var result = _mapper.Map<List<DtoRequestCreditMemo>>(list);


                return new OperationResponse<DtoPagination<DtoRequestCreditMemo>>(new DtoPagination<DtoRequestCreditMemo>
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

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestCreditMemo model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            CreditMemo creditModel = null;

            try
             {
                if (model.Id == 0)
                {
        
                    creditModel = _mapper.Map<CreditMemo>(model);
                    creditModel.CompanyId = requestedBy.CompanyId;

                    if (creditModel.CustomerId == 0)
                    {
                        var user = await _contextSql.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.Name == "Admin");
                        creditModel.CustomerId = user?.Id;
                    }

                    foreach (CreditMemoDetail creditMemo in creditModel.CreditMemoDetail) { if (creditMemo.ProductId <= 0) { creditMemo.ProductId = -1;} }

                    creditModel.InvoiceId = creditModel.InvoiceId == 0 ? null : creditModel.InvoiceId;

                    var regex = new Regex(@"^-?[0-9][0-9,\.]+$");

                    //Verifico que el DNI O CUIT no tenga letras
                    if (!regex.IsMatch(model.CustomerCuit))
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, El CUIT/DNI tiene que ser numerico"));
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

                    //Verifico que el DNI tenga mayor a 7 caracteres y menor a 9
                    if (model.Type == 2 && model.CustomerCuit.Length < 7 || model.CustomerCuit.Length > 8 && model.CustomerCuit.Length != 11)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, verifique DNI"));
                    }

                    await _contextSql.CreditMemo.AddAsync(creditModel, ct).ConfigureAwait(false);
                }
                

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                transaction.Commit();
                return Ok(new IdResponse<long>(creditModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO)));
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestCreditMemo model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                if (model.Id == 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "La nota de credito no tiene ID"));
                }
                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }        

        public async Task<OperationResponse<IEnumerable<DtoResponseIntegrationLogCredit>>> GetIntegrationLogById(long Id, CancellationToken ct = default)
        {
            try
            {
                IEnumerable<DtoResponseIntegrationLogCredit> logs = new List<DtoResponseIntegrationLogCredit>();
                string invoicesScript = SqlScripts.GetIntegrationLogCreditById;
                using (var connection = new SqlConnection(ConnectionString))
                {
                    logs = await connection.QueryAsync<DtoResponseIntegrationLogCredit>
                        (invoicesScript, param: new { @id = Id });
                }
                return new OperationResponse<IEnumerable<DtoResponseIntegrationLogCredit>>(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex: ex);
                throw;
            }
        }
    }
}
