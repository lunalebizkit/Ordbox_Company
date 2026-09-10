
using AutoMapper;
using Dapper;
using Ordbox.Domain;
using Ordbox.Domain.Enum;
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
    public class ProductService : BaseService
    {
        public ProductService(/*ImageService imageService,*/ ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
            base(logger, context, maper, configuration)

        { }

        public async Task<OperationResponse<DtoResponseProduct>> GetById(long id, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {

                    var product = connection.QuerySingle<DtoResponseProduct>(SqlScripts.GetCompleteProductById, new { @productid = id, @companyid = requestedBy.CompanyId });

                    if (product == null)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<DtoResponseProduct>(new OperationExceptions("000", $"Producto no encontrado ID: {id}"));
                    };
                    var category = connection.Query<DtoGenericResponse>(SqlScripts.GetCategoryByIdForList, new { @productid = id }).ToList();
                    var brand = connection.Query<DtoGenericResponse>(SqlScripts.GetBrandByIdForList, new { @productid = id }).ToList();
                    var supplier = connection.Query<DtoGenericResponse>(SqlScripts.GetSupplierByIdForList, new { @productid = id }).ToList();
                    product.Category = category;
                    product.Brand = brand;
                    product.Supplier = supplier;
                    return new OperationResponse<DtoResponseProduct>(product);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> Add(DtoRequestAddProduct model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                model.Id = 0;
                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestAddProduct model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                //Log de Productos
                _logger.LogInfo(ErrorsCodes.C_RQ_PRODUCT_REQUEST, model);

                var id = model.Id;
                if (model.Id == 0)
                {
                    var productModel = _mapper.Map<Product>(model);
                    productModel.CompanyId = requestedBy.CompanyId;
                    await _contextSql.Products.AddAsync(productModel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldProduct = await _contextSql.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
                    if (oldProduct != null)
                    {
                        if (oldProduct.IsDeleted) { model.IsDeleted = true; }
                        oldProduct = _mapper.Map(model, oldProduct);
                        oldProduct.Id = model.Id;
                        oldProduct.CompanyId = requestedBy.CompanyId;
                    }
                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoResponseProduct>>> List(RequestPaginatedData<ProductFilter> request, RequestedBy requestedBy)
        {
            try
            {
                var query = _contextSql
                                    .Products
                                    .AsNoTracking()
                                      .Include(p => p.Category)
                                    .Include(p => p.Brand)
                                    .Include(p => p.Supplier)
                                    .Where(p => !p.IsDeleted && p.Id > 0)
                                    .Where(p => p.CompanyId == requestedBy.CompanyId &&
                                     (string.IsNullOrEmpty(request.Filter.Product) || p.Description.ToLower().Contains(request.Filter.Product.ToLower())) &&
                                    (!request.Filter.Brand.HasValue || request.Filter.Brand == 0 || p.BrandId == request.Filter.Brand) &&
                                    (!request.Filter.Category.HasValue || request.Filter.Category == 0 || p.CategoryId == request.Filter.Category) &&
                                    (string.IsNullOrEmpty(request.Filter.Code) || p.Code.ToLower().Contains(request.Filter.Code.ToLower())) &&
                                    (string.IsNullOrEmpty(request.Filter.BarCode) || p.BarCode.ToLower().Contains(request.Filter.BarCode.ToLower())) &&
                                    (request.Filter.Supplier.Count == 0 || request.Filter.Supplier.Contains(p.SupplierId))
                                    );

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);


                var dto = _mapper.Map<List<DtoResponseProduct>>(list);


                return new OperationResponse<DtoPagination<DtoResponseProduct>>(new DtoPagination<DtoResponseProduct>
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

        public async Task<OperationResponse<DtoPagination<DtoResponseProduct>>> ListInactive(RequestPaginatedData<ProductFilter> request, RequestedBy requestedBy)
        {
            try
            {
                var query = _contextSql
                                    .Products.Where(p => p.IsDeleted)
                                    .AsNoTracking()
                                      .Include(p => p.Category)
                                    .Include(p => p.Brand)
                                    .Include(p => p.Supplier)
                                    .Where(p => p.CompanyId == requestedBy.CompanyId && (!string.IsNullOrEmpty(request.Filter.Product) ? p.Description.ToLower().Contains(request.Filter.Product) : true) &&
                                    ((request.Filter.Brand.HasValue && request.Filter.Brand != 0) ? p.BrandId == request.Filter.Brand : true) &&
                                     ((request.Filter.Category.HasValue && request.Filter.Category != 0) ? p.CategoryId == request.Filter.Category : true) &&
                                     (!string.IsNullOrEmpty(request.Filter.Product) ? p.Description.ToLower().Contains(request.Filter.Product) : true) &&
                                     (!string.IsNullOrEmpty(request.Filter.Code) ? p.Code.ToLower().Contains(request.Filter.Code) : true) &&
                                     (!string.IsNullOrEmpty(request.Filter.BarCode) ? p.BarCode.ToLower().Contains(request.Filter.BarCode) : true)
                                    && p.IsDeleted && p.Id > 0);

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);


                var dto = _mapper.Map<List<DtoResponseProduct>>(list);


                return new OperationResponse<DtoPagination<DtoResponseProduct>>(new DtoPagination<DtoResponseProduct>
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

        //Elimianr Producto
        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, RequestedBy requestedBy, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    var product = connection.Query(SqlScripts.GetProductById, new { @productid = id, @companyid = requestedBy.CompanyId }).FirstOrDefault();

                    if (product != null)
                    {
                        var savedProduct = await _contextSql.Products.FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == requestedBy.CompanyId, ct).ConfigureAwait(false);

                        if (savedProduct != null)
                        {
                            savedProduct.IsDeleted = true;

                            await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                            return Ok(new IdResponse<long>(id));
                        }
                        else
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND));
                            return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_004_ELEMENT_NOT_FOUND, ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND)));
                        }
                    }
                    else
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION));
                        return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_010_ERROR_EXCEPTION, "El producto tiene marcas asociadas"));
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                    throw;
                }
            }
        }

        //Activar Producto
        public async Task<OperationResponse<IdResponse<long>>> Active(long id, RequestedBy requestedBy, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    var product = connection.Query(SqlScripts.GetProductById, new { @productid = id, @companyid = requestedBy.CompanyId }).FirstOrDefault();

                    if (product != null)
                    {
                        var savedProduct = await _contextSql.Products.FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == requestedBy.CompanyId, ct).ConfigureAwait(false);

                        if (savedProduct != null)
                        {
                            savedProduct.IsDeleted = false;

                            await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                            return Ok(new IdResponse<long>(id));
                        }
                        return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_004_ELEMENT_NOT_FOUND, ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND)));
                    }
                    else
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND));
                        return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_004_ELEMENT_NOT_FOUND, ErrorsMessages.GetMessage(ErrorsCodes.C_004_ELEMENT_NOT_FOUND)));

                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                    throw;
                }
            }
        }

        //Servicio que utlizamos para filtrar en UPDATEPRICEPRODUCT
        public async Task<OperationResponse<DtoPagination<DtoResponseProduct>>> ListProduct(RequestPaginatedData<ProductFilter> request)
        {
            try
            {
                var query = _contextSql
                                    .Products
                                    .AsNoTracking()
                                    .Include(p => p.Category)
                                    .Include(p => p.Brand)
                                    .Include(p => p.Supplier)
                                    .Where(p => (!string.IsNullOrEmpty(request.Filter.Product) ? p.Description.ToLower().Contains(request.Filter.Product) : true)
                                    &&
                                    ((request.Filter.Brand.HasValue && request.Filter.Brand != 0) ? p.BrandId == request.Filter.Brand : true)
                                    &&
                                     ((request.Filter.Category.HasValue && request.Filter.Category != 0) ? p.CategoryId == request.Filter.Category : true)
                                    &&
                                     (request.Filter.Supplier.Count > 0 ? request.Filter.Supplier.Contains(p.SupplierId) : true));

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);


                var dto = _mapper.Map<List<DtoResponseProduct>>(list);


                return new OperationResponse<DtoPagination<DtoResponseProduct>>(new DtoPagination<DtoResponseProduct>
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
        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestAddProduct model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                if (model.Id == 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "El prodcuto no tiene ID"));
                }
                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<bool>> UpdatePriceProduct(DtoUpdatePriceProduct model, CancellationToken ct = default)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<bool>("000", "El producto no tiene Id");
                }
                var productos = _contextSql
                                   .Products
                                   .Include(p => p.Category)
                                   .Include(p => p.Brand)
                                   .Include(p => p.Supplier)
                                   .Where(p => (!String.IsNullOrEmpty(model.Product) ? p.Description.ToLower().Contains(model.Product) : true)
                                   &&
                                   ((model.Brand.HasValue && model.Brand != 0) ? p.BrandId == model.Brand : true)
                                   &&
                                    ((model.Category.HasValue && model.Category != 0) ? p.CategoryId == model.Category : true)
                                   &&
                                    (model.Supplier.Count > 0 ? model.Supplier.Contains(p.SupplierId) : true));


                foreach (var item in productos)
                {
                    switch (model.IdPrice)
                    {
                        case (int)EPriceProduct.PurchasePrice:
                        case (int)EPriceProduct.Percentage:
                            item.UpdateSalePrice(model.Value, model.IdPrice == (int)EPriceProduct.Percentage);
                            break;
                        case (int)EPriceProduct.CardSalePercentage:
                        case (int)EPriceProduct.CashSalePercentage:
                        case (int)EPriceProduct.SalePercentage:
                            item.UpdatePrecentage(model.Value, model.IdPrice);
                            break;
                    }

                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                return Ok<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoResponseProductReportTotal>> GetProductReport(RequestedBy requestedBy)
        {
            DtoResponseProductReportTotal productReportTotal = new DtoResponseProductReportTotal();
            IEnumerable<DtoResponseProductReport> productReport = new List<DtoResponseProductReport>();
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    productReport = await connection.QueryAsync<DtoResponseProductReport>(SqlScripts.GetProductReport, new { @companyid = requestedBy.CompanyId });
                }
                if (productReport != null && productReport.Count() > 0)
                {
                    productReportTotal.Total = productReport?.Sum(p => (p.Quantity * p.Purchase_Price)) ?? 0m;
                    productReportTotal.Date = DateTime.Now;
                    productReportTotal.Products = productReport.ToList();
                }


                return new OperationResponse<DtoResponseProductReportTotal>(productReportTotal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<DtoResponseProductReportTotal>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ex?.Message.ToString()));
            }
        }

        public async Task UpdateProductStockById(long productId, int stock, RequestedBy requestedBy)
        {
            var parameters = new
            {
                recievedquantity = stock,
                productid = productId,
                companyid = requestedBy.CompanyId,
            
            };

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var rowAffected = await connection.ExecuteAsync(SqlScripts.UpdateProductStockById, parameters,transaction: transaction,commandType: System.Data.CommandType.Text);

                        if (rowAffected == 0)
                        {
                            _logger.LogWarning($"No se encontró el producto con ID {productId} para actualizar el stock.");
                        }

                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ErrorsCodes.C_010_ERROR_EXCEPTION, ex: ex);
                        throw;
                    }
                }
            }
        }
    }
}
