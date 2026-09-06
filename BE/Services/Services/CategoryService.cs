

using AutoMapper;
using Dapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Scripts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class CategoryService : BaseService
    {
        public CategoryService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
            base(logger, context, maper, configuration)
        { }

        //Get Categoria
        public async Task<OperationResponse<DtoResponseCategory>> GetById(long id)
        {
            try
            {
                var categoria = GetCategoryById(id);
                if (categoria == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseCategory>(new OperationExceptions("000", $"Usuario no encontrado Id:{id}"));
                }                

                return new OperationResponse<DtoResponseCategory>(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
            
        }
        public async Task<OperationResponse<IdResponse<long>>> Add(DtoResponseCategory model, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoResponseCategory model, CancellationToken ct = default)
        {
            try
            {
                var countCategory = await _contextSql
                                .Category
                                .AsNoTracking()
                                .CountAsync(p => p.Description.ToUpper() == model.Description.ToUpper() && p.Id != model.Id, ct);
                if (countCategory > 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error <IdResponse<long>> (new OperationExceptions("009", "Ya existe una categoria con ese nombre")); 
                }

                var categoryModel = new Category()
                {
                    Id = model.Id,
                    Description = model.Description
                };
                if (categoryModel.Id == 0)
                {
                    await _contextSql.Category.AddAsync(categoryModel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldModel = await _contextSql
                                    .Category
                                    .FirstAsync(p => p.Id == categoryModel.Id)
                                    .ConfigureAwait(false);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;

                    _contextSql.Category.Update(categoryModel);
                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(categoryModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoResponseCategory model, CancellationToken ct = default)
        {
            if (model.Id <= 0)
            {
                _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                return Error<IdResponse<long>>(new OperationExceptions("000", "La Categoria no tiene ID"));
            }

            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }
        public async Task<OperationResponse<DtoPagination<DtoResponseCategory>>> ListCategory(RequestPaginatedData<string> request)
        {
            try
            {
                var query = _contextSql
                                    .Category
                                    .AsNoTracking()
                                    .Where(p => ((p.Description.ToLower().Contains(request.Filter ?? "")) && p.Id > 0));

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)                                
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                var dto = _mapper.Map<List<DtoResponseCategory>>(list);

                return new OperationResponse<DtoPagination<DtoResponseCategory>>(new DtoPagination<DtoResponseCategory>
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
        //Elimianr Categoría
        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    var category = connection.Query(SqlScripts.GetCountCategoryById, new { @categoryid = id }).FirstOrDefault();

                    if (category != null)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION));
                        return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, "La Categoría tiene productos asociados"));
                    }
                    else
                    {
                        var savedCategory = await _contextSql.Category.FirstOrDefaultAsync(p => p.Id == id, ct).ConfigureAwait(false);

                        if (savedCategory != null)
                        {
                            _contextSql.Category.Remove(savedCategory);

                            await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                            return Ok(new IdResponse<long>(id));
                        }
                        else
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND));
                            return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_004_ELEMENT_NOT_FOUND, ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND)));
                        }
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                    throw;
                }
            }
        }
        #region Private

        private DtoResponseCategory? GetCategoryById(long id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<DtoResponseCategory>(SqlScripts.GetCategoryById, new { @id = id }).FirstOrDefault();
            }
        }

        #endregion
    }
}
