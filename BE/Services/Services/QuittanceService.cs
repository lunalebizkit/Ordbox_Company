using AutoMapper;
using Dapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Scripts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain.Model.Extensions;

namespace Ordbox.Services.Services
{
    public class QuittanceService : BaseService
    {
        public QuittanceService(ErrorManager logger, DBContext context, IMapper mapper, IConfiguration configuration) : base(logger, context, mapper, configuration)
        {
        }

        public async Task<OperationResponse<DtoResponseQuittance>> GetById(long id, RequestedBy requestedBy)
        {
            try
            {

                var model = await _contextSql
                               .Quittance
                               .Include(x => x.QuittanceDetails)
                               .Include(x => x.QuittanceProductDetails)
                               .AsNoTracking()
                               .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == requestedBy.CompanyId)
                               .ConfigureAwait(false);

                if (model == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseQuittance>(new OperationExceptions("000", $"Recibo no encontrado {id}"));
                }

                var result = _mapper.Map<DtoResponseQuittance>(model);


                return new OperationResponse<DtoResponseQuittance>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoResponseQuittance>>> ListQuittance(RequestPaginatedData<SpecificFilter> request, RequestedBy requestedBy)
        {

            try
            {
                var query = _contextSql
                                    .Quittance
                                    .AsNoTracking()
                                    .Where(p => p.CompanyId == requestedBy.CompanyId && (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.CustomerCuit.ToLower().Contains(request.Filter.Cuit) : true)
                                     && ((request.Filter.Number.HasValue && request.Filter.Number != 0) ? p.Id == request.Filter.Number : true)
                                     && ( (!request.Filter.Date.Contains("") || request.Filter.Date != null) ? p.DateTime.Date.ToString().Contains(request.Filter.Date) : true)
                                     && (!string.IsNullOrEmpty(request.Filter.CustomerName) ? p.CustomerName.ToLower().Contains(request.Filter.CustomerName) : true)
                                     );

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderByDescending(p => p.Id).ThenBy(p => p.QuittanceNumber)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                var dto = _mapper.Map<List<DtoResponseQuittance>>(list);

                return new OperationResponse<DtoPagination<DtoResponseQuittance>>(new DtoPagination<DtoResponseQuittance>
                {
                    Data = dto,
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

        public async Task<OperationResponse<IdResponse<long>>> NewQuittance(DtoRequestQuittance model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);
        }

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestQuittance model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                var newModel = _mapper.Map<Quittance>(model);
                newModel.CompanyId = requestedBy.CompanyId;

                if (newModel.Id == 0)
                {
                    if(newModel.QuittanceProductDetails != null && newModel.QuittanceProductDetails.Any())
                    {
                        foreach (var detail in newModel.QuittanceProductDetails)
                        {                        
                            if (detail.ProductId < 0)
                            {
                                detail.ProductId = -1;
                            }
                        }
                    }

                    await _contextSql.Quittance.AddAsync(newModel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldQuittance = await _contextSql
                                    .Quittance
                                    .Include(x => x.QuittanceDetails)
                                    .Include(x => x.QuittanceProductDetails)
                                    .FirstAsync(p => p.Id == model.Id && p.CompanyId == requestedBy.CompanyId)
                                    .ConfigureAwait(false);

                    _contextSql.QuittanceDetails.RemoveRange(oldQuittance.QuittanceDetails);
                    _contextSql.QuittanceProductDetails.RemoveRange(oldQuittance.QuittanceProductDetails);

                    _contextSql.Entry(oldQuittance).State = EntityState.Detached;
                    newModel.Id = oldQuittance.Id;
                                      
                    _contextSql.Update(newModel);

                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(newModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO)));
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestQuittance model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "El remito no tiene ID"));
                }
                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

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
    }
}
