using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class PeriodService : BaseService
    {
        public PeriodService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
           base(logger, context, maper, configuration)

        { }
        public async Task<OperationResponse<DtoResponsePeriod>> GetById(long id)
        {
            try
            {
                var periodo = await _contextSql
                                   .Periods
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(p => p.Id == id)
                                   .ConfigureAwait(false);
                if (periodo == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponsePeriod>(new OperationExceptions("000", $"Periodo no encontrada Id: {id}"));

                }

                var result = _mapper.Map<DtoResponsePeriod>(periodo);

                return new OperationResponse<DtoResponsePeriod>(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Add(DtoRequestPeriod model, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestPeriod model, CancellationToken ct = default)
        {
            try
            {
                var countPeriods = await _contextSql
                                    .Periods
                                    .AsNoTracking()
                                    .CountAsync(p => p.InitPeriod.Date == model.InitPeriod.Date && p.Id != model.Id, ct);

                var activePeriods = _contextSql
                                    .Periods
                                    .AsNoTracking()
                                    .Where(p => (p.Status == true) || (p.EndPeriod.Date >= model.InitPeriod.Date));

                if (countPeriods > 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "Ya existe este periodo"));
                }

                var NewModel = _mapper.Map<Period>(model);

                if (NewModel.Id == 0)
                {
                    if (activePeriods.Count() > 0)
                    {
                        return Error<IdResponse<long>>(new OperationExceptions("009", "hay periodos activos"));
                    }
                    else
                    {
                        await _contextSql.Periods.AddAsync(NewModel, ct).ConfigureAwait(false);
                    }

                }

                else
                {
                    var oldModel = await _contextSql
                                    .Periods
                                    .FirstAsync(p => p.Id == NewModel.Id)
                                    .ConfigureAwait(false);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;
                    _contextSql.Periods.Update(NewModel);
                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(NewModel.Id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestPeriod model, CancellationToken ct = default)
        {

            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "Periodo no tiene ID"));
                }

                return await AddOrUpdate(model, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoResponsePeriod>>> ListPeriods(RequestPaginatedData<string> request)
        {

            try
            {
                var query = _contextSql
                                    .Periods
                                    .AsNoTracking()
                                    .Where(p => (!request.Filter.Contains("") || request.Filter != null) ? p.InitPeriod.Date.ToString().Contains(request.Filter) : true &&
                                    (!request.Filter.Contains("") || request.Filter != null) ? p.EndPeriod.Date.ToString().Contains(request.Filter) : true);

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderByDescending(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                var dto = _mapper.Map<List<DtoResponsePeriod>>(list);

                return new OperationResponse<DtoPagination<DtoResponsePeriod>>(new DtoPagination<DtoResponsePeriod>
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
        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, CancellationToken ct = default)
        {
            try
            {
                var period = await _contextSql
                                             .Periods
                                             .FirstOrDefaultAsync(p => p.Id == id && p.Status, ct)
                                             .ConfigureAwait(false);
                if (period != null)
                {
                    period.Status = false;

                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "No existe periodo"));
                }

                return Ok(new IdResponse<long>(id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<bool>> ActivePeriod(DateTime date)
        {
            try
            {
                var periodActive = _contextSql
                                 .Periods
                                 .AsNoTracking()
                                 .FirstOrDefault(p => (p.Status == true) && (p.InitPeriod.Date <= date.Date) && (p.EndPeriod.Date >= date.Date));


                if (periodActive == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>("000", "no se ha recibo parametro");
                }
                else
                {
                    return Ok<bool>(true);
                }
           }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<DtoPagination<DtoResponsePeriod>>> SelectedPeriod(RequestPaginatedData<PeriodFilter> request)
        {
            try
            {
                var selectedPeriod = _contextSql
                                 .Periods
                                 .AsNoTracking()
                                 .Where(p => (p.InitPeriod.Date <= request.Filter.Date) && (p.EndPeriod.Date >= request.Filter.Date))
                                 ;
                var count = await selectedPeriod.CountAsync().ConfigureAwait(false);

                var list = await selectedPeriod.OrderByDescending(p => p.InitPeriod)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                var dto = _mapper.Map<List<DtoResponsePeriod>>(selectedPeriod);

                    return new OperationResponse<DtoPagination<DtoResponsePeriod>>(new DtoPagination<DtoResponsePeriod>
                    {
                        Data = dto,
                        PageSize = request.PageSize,
                        TotalCount = 1
                    });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task CreateOrReplaceMonthlyPeriod(CancellationToken ct = default)
        {
            try
            {
                var today = DateTime.Today;
                var initPeriod = new DateTime(today.Year, today.Month, 1);
                var endPeriod = initPeriod.AddMonths(1).AddDays(-1);

                var activePeriod = await _contextSql.Periods
                    .FirstOrDefaultAsync(p => p.Status == true, ct)
                    .ConfigureAwait(false);

                if (activePeriod == null || activePeriod.EndPeriod < today)
                {
                    // Si hay activo y ya venció, lo cerramos
                    if (activePeriod != null)
                    {
                        activePeriod.Status = false;
                        _contextSql.Periods.Update(activePeriod);
                    }

                    Period newPeriod = new()
                    {
                        InitPeriod = initPeriod,
                        EndPeriod = endPeriod,
                        Status = true
                    };

                    await _contextSql.Periods.AddAsync(newPeriod, ct);
                    await _contextSql.SaveChangesAsync(ct);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear o reemplazar periodo mensual", ex);
                throw;
            }
        }
    }
}
