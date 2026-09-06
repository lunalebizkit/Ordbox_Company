using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Scripts;


namespace Ordbox.Services.Services
{
    public class BudgetService : BaseService
    {
        public BudgetService(ErrorManager logger, DBContext context, IMapper mapper, IConfiguration config) : base(logger, context, mapper, config)
        {
        }

        public async Task<OperationResponse<DtoResponseBudget>> GetById(long id)
        {
            try
            {

                var model = GetBudgetById(id);

                if (model == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseBudget>(new OperationExceptions("000", $"Presupuesto no encontrado {id}"));
                }

                return new OperationResponse<DtoResponseBudget>(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> New(DtoRequestBudget model, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }

        public async Task<OperationResponse<IdResponse<long>>> Add(DtoRequestBudget model, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestBudget model, CancellationToken ct = default)
        {
            try
            {
                var newModel = _mapper.Map<Budget>(model);

                foreach (BudgetDetail budgetDetail in newModel.BudgetDetails)
                {
                    if (budgetDetail.ProductId <= 0) { budgetDetail.ProductId = -1; }
                }

                if (newModel.Id == 0)
                {

                    await _contextSql.Budgets.AddAsync(newModel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldModel = await _contextSql
                                    .Budgets
                                    .Include(x => x.BudgetDetails)
                                    .FirstAsync(p => p.Id == model.Id)
                                    .ConfigureAwait(false);

                    _contextSql.BudgetDetails.RemoveRange(oldModel.BudgetDetails);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;

                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                    newModel.Total = 0;

                    var newModel2 = _mapper.Map<Budget>(newModel);
                    foreach (var item in newModel.BudgetDetails)
                    {
                        item.Id = 0;
                        newModel.Total += item.Price * item.Quantity;
                        oldModel.BudgetDetails.Add(item);
                    }

                    _contextSql.Attach(newModel2);

                    _contextSql.Update(newModel2);


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

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestBudget model, CancellationToken ct = default)
        {

            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "La Marca no tiene ID"));
                }

                return await AddOrUpdate(model, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoResponseBudget>>> ListBudget(RequestPaginatedData<SpecificFilter> request)
        {

            try
            {
                var query = _contextSql
                                    .Budgets
                                    .Include(p => p.User)
                                    .AsNoTracking()
                                    .Where(p => !p.IsInactive &&
                                    (request.Filter.Number.HasValue && request.Filter.Number != 0 ? p.BudgetNumber == request.Filter.Number : true) &&
                                    (!string.IsNullOrEmpty(request.Filter.Date) ? p.DateTime.Date.ToString().Contains(request.Filter.Date) : true) &&
                                    (!string.IsNullOrEmpty(request.Filter.CustomerName) ? p.CustomerName.ToLower().Contains(request.Filter.CustomerName.ToLower()) : true)
                                     );

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderByDescending(p => p.DateTime).ThenBy(p => p.BudgetNumber)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                var dto = _mapper.Map<List<DtoResponseBudget>>(list);

                return new OperationResponse<DtoPagination<DtoResponseBudget>>(new DtoPagination<DtoResponseBudget>
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
                var model = await _contextSql
                                             .Budgets
                                             .FirstOrDefaultAsync(p => p.Id == id && !p.IsInactive, ct)
                                              .ConfigureAwait(false);

                if (model != null)
                {
                    model.IsInactive = true;

                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION));
                    return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION)));
                }

                return Ok(new IdResponse<long>(id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        #region Private

        private DtoResponseBudget? GetBudgetById(long Id)
        {
            using var connection = new SqlConnection(ConnectionString);

            var budgetDictionary = new Dictionary<long, DtoResponseBudget>();

            var result = connection.Query<DtoResponseBudget, DtoResponseBudgetDetail, DtoResponseBudget>(
                sql: SqlScripts.GetBudgetById,
                map: (budget, detail) =>
                {
                    if (!budgetDictionary.TryGetValue(budget.Id, out var budgetEntry))
                    {
                        budgetEntry = budget;
                        budgetEntry.BudgetDetails = new List<DtoResponseBudgetDetail>();
                        budgetDictionary.Add(budgetEntry.Id, budgetEntry);
                    }

                    if (detail != null)
                    {
                        budgetEntry.BudgetDetails.Add(detail);
                    }

                    return budgetEntry;
                }, param: new { @id = Id }, splitOn: "Id");

            return budgetDictionary.Values.SingleOrDefault();
        }

        #endregion
    }

}

