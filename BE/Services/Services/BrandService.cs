

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
using Ordbox.Domain.Model.Extensions;

namespace Ordbox.Services.Services
{
    public class BrandService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) : BaseService(logger, context, maper, configuration)
    {
        public async Task<OperationResponse<DtoResponseBrand>> GetById(long id, RequestedBy requestedBy)
        {
            try
            {
                var marca = GetBrandById(id, requestedBy);
                if (marca == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseBrand>(new OperationExceptions("000", $"Marca no encontrada Id: {id}"));
                }

                var result = _mapper.Map<DtoResponseBrand>(marca);

                return new OperationResponse<DtoResponseBrand>(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> Add(DtoResponseBrand model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);
        }
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoResponseBrand model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                model.CompanyId = requestedBy.CompanyId;

                var countBrands = await _contextSql
                                    .Brands
                                    .AsNoTracking()
                                    .CountAsync(p => p.Description.ToUpper() == model.Description.ToUpper() && p.Id != model.Id && p.CompanyId == model.CompanyId, ct);
                if (countBrands > 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "Ya existe una marca con ese nombre"));
                }

                var brandModel = _mapper.Map<Brand>(model);

                if (brandModel.Id == 0)
                {
                    await _contextSql.Brands.AddAsync(brandModel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldModel = await _contextSql
                                    .Brands
                                    .FirstAsync(p => p.Id == brandModel.Id)
                                    .ConfigureAwait(false);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;

                    _contextSql.Brands.Update(brandModel);
                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(brandModel.Id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoResponseBrand model, RequestedBy requestedBy, CancellationToken ct = default)
        {

            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "La Marca no tiene ID"));
                }

                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoResponseBrand>>> ListBrands(RequestPaginatedData<string> request, RequestedBy requestedBy)
        {
            try
            {
                var query = _contextSql
                                    .Brands
                                    .AsNoTracking()
                                    .Where(p => ((p.Description.ToLower().Contains(request.Filter ?? "")) && p.Id > 0 && p.CompanyId == requestedBy.CompanyId));

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                var dto = _mapper.Map<List<DtoResponseBrand>>(list);

                return new OperationResponse<DtoPagination<DtoResponseBrand>>(new DtoPagination<DtoResponseBrand>
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
        //Elimianr Marca
        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, RequestedBy requestedBy, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    var brand = connection.Query(SqlScripts.GetCountBrandById, new { @brandid = id, @companyid = requestedBy.CompanyId }).FirstOrDefault();

                    if (brand != null)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION));
                        return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, "La Marca tiene productos asociados"));
                    }
                    else
                    {
                        var savedBrand = await _contextSql.Brands.FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == requestedBy.CompanyId, ct).ConfigureAwait(false);

                        if (savedBrand != null)
                        {
                            _contextSql.Brands.Remove(savedBrand);

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

        private Brand? GetBrandById(long id, RequestedBy requestedBy)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<Brand>(SqlScripts.GetBrandById, new { @id = id, @companyid = requestedBy.CompanyId }).FirstOrDefault();
            }
        }

        #endregion

    }
}
