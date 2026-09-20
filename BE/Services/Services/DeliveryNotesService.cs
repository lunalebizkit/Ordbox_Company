using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain.Model.Extensions;

namespace Ordbox.Services.Services
{
    public class DeliveryNotesService : BaseService
    {
        public DeliveryNotesService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
            base(logger, context, maper, configuration)
        { }
        public async Task<OperationResponse<DtoRequestDeliveryNotes>> GetById(long id, RequestedBy requestedBy)
        {
            try
            {
                var remitos = await _contextSql
                                   .DeliveryNotes
                                   .Include(x => x.DeliveryNotesDetails)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == requestedBy.CompanyId)
                                   .ConfigureAwait(false);
                if ( remitos == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoRequestDeliveryNotes>(new OperationExceptions("000", $"Remito no encontrado {id}"));
                }

                var result = _mapper.Map<DtoRequestDeliveryNotes>(remitos);


                return new OperationResponse<DtoRequestDeliveryNotes>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> NewDeliveryNotes(DtoRequestDeliveryNotes model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);
        }

        public async Task<OperationResponse<DtoPagination<DtoRequestDeliveryNotes>>> ListDeliveryNotes(RequestPaginatedData<SpecificFilter> request, RequestedBy requestedBy)
        {
            try
            {
                var query = _contextSql
                                    .DeliveryNotes
                                    .AsNoTracking()
                                    .Where(p => p.CompanyId == requestedBy.CompanyId && (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.SupplierCuit.ToLower().Contains(request.Filter.Cuit) : true)
                                        && ((request.Filter.Number.HasValue && request.Filter.Number != 0) ? p.Id == request.Filter.Number : true)
                                        && ((!request.Filter.Date.Contains("") || request.Filter.Date != null) ? p.DateTime.Date.ToString().Contains(request.Filter.Date) : true)
                                        &&  (!string.IsNullOrEmpty(request.Filter.CustomerName) ? p.SupplierName.ToLower().Contains(request.Filter.CustomerName) : true)
                                     );

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderByDescending(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                var result = _mapper.Map<List<DtoRequestDeliveryNotes>>(list);


                return new OperationResponse<DtoPagination<DtoRequestDeliveryNotes>>(new DtoPagination<DtoRequestDeliveryNotes>
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

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestDeliveryNotes model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {

                var newModel = _mapper.Map<DeliveryNotes>(model);
                newModel.CompanyId = requestedBy.CompanyId;

                foreach (DeliveryNotesDetails item in newModel.DeliveryNotesDetails) { if (item.ProductId <= 0) { item.ProductId = -1; } }

                if (newModel.Id == 0)
                {
                    await _contextSql.DeliveryNotes.AddAsync(newModel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldModel = await _contextSql
                                    .DeliveryNotes.Include(y => y.DeliveryNotesDetails)
                                    .FirstAsync(p => p.Id == model.Id && p.CompanyId == requestedBy.CompanyId)
                                    .ConfigureAwait(false);

                    _contextSql.DeliveryNotesDetails.RemoveRange(oldModel.DeliveryNotesDetails);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;
                    _contextSql.DeliveryNotes.Update(newModel);
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

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestDeliveryNotes model, RequestedBy requestedBy, CancellationToken ct = default)
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
    }
}