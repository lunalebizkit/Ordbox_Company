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
using Ordbox.Services.Scripts;

namespace Ordbox.Services.Services
{
    public class ReceiptService : BaseService
    {
        public ReceiptService(ErrorManager logger , DBContext context , IMapper maper, IConfiguration configuration) : 
            base(logger , context , maper, configuration) 
        { }

        //Get Receipt
        public async Task<OperationResponse<DtoRequestReceipt>> GetById(long id)
        {
            try
            {
                var receipt = await _contextSql
                                    .Receipts
                                    .Include(x => x.ReceiptDetails)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync( c => c.Id == id)
                                    .ConfigureAwait(false);

                if(receipt == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoRequestReceipt>(new OperationExceptions("000", $"Comprobante no encontrado Id: {id}"));
                }

                var result = _mapper.Map<DtoRequestReceipt>(receipt);

                result.Iva10 = 0;
                result.Iva21 = 0;
                result.Iva27 = 0;
                foreach (var item in result.ReceiptDetails)
                {
                    result.Iva10 += ((decimal)item.Iva == (decimal)10.5) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.105m : 0;
                    result.Iva21 += ((decimal)item.Iva == (decimal)21) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.21m : 0;
                    result.Iva27 += ((decimal)item.Iva == (decimal)27) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.27m : 0;
                }
                return new OperationResponse<DtoRequestReceipt>(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }

        public async Task<OperationResponse<IdResponse<long>>> NewReceipt(DtoRequestReceipt model, CancellationToken ct = default)
        {
            try
            {
                model.Id = 0;
                if (String.IsNullOrEmpty(model.SupplierName))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "Datos incompletos")); ;
                }
                return await AddOrUpdate(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoRequestListReceipt>>> ListReceipt(RequestPaginatedData<SpecificFilter> request, long companyId)
        {

            try
            {
                List<DtoRequestListReceipt> dtoRequests = new List<DtoRequestListReceipt>();
                int count = 0;
                using (var connection = new SqlConnection(ConnectionString))
                {
                    dtoRequests = connection.Query<DtoRequestListReceipt>(SqlScripts.GetReceiptList, new
                    {
                        companyid = companyId,
                        cuit = request.Filter.Cuit,
                        number = request.Filter.Number,
                        date = request.Filter.Date,
                        customername = request.Filter.CustomerName,
                        Page = request.Page,
                        PageSize = request.PageSize
                    }).ToList();

                    count = connection.QuerySingle<int>(SqlScripts.GetReceiptListCount, new
                    {
                        companyid = companyId,
                        cuit = request.Filter.Cuit,
                        number = request.Filter.Number,
                        date = request.Filter.Date,
                        customername = request.Filter.CustomerName
                    });

                }

                //    var query = _contextSql
                //                    .Receipts
                //                    .AsNoTracking()
                //                    .Include(x => x.User).ThenInclude(x => x.Company)
                //                     .Where(p => p.User.CompanyId == companyId && (!p.IsInactive) && (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.SupplierCuit.ToLower().Contains(request.Filter.Cuit) : true)
                //                     && ((request.Filter.Number.HasValue && request.Filter.Number != 0) ? p.ReceiptNumber == request.Filter.Number : true)
                //                      &&
                //                     ((!request.Filter.Date.Contains("") || request.Filter.Date != null) ? p.DateTime.Date.ToString().Contains(request.Filter.Date) : true)
                //                      &&
                //                     (!string.IsNullOrEmpty(request.Filter.CustomerName) ? p.SupplierName.ToLower().Contains(request.Filter.CustomerName) : true)
                //                     );

                //var count = await query.CountAsync().ConfigureAwait(false);

                //var list = await query.OrderByDescending(p => p.DateTime)
                //                      .Skip(request.Page * request.PageSize)
                //                      .Take(request.PageSize)
                //                      .ToListAsync()
                //                      .ConfigureAwait(false);

                //var dto = _mapper.Map<List<DtoRequestListReceipt>>(list);

                return new OperationResponse<DtoPagination<DtoRequestListReceipt>>(new DtoPagination<DtoRequestListReceipt>
                {
                    Data = dtoRequests,
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

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestReceipt model, CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            var receiptModel = _mapper.Map<Receipt>(model);
            var productDetail = new Product();

            try
            {
                if (receiptModel.Id == 0)
                {
                    if (receiptModel.SupplierId == 0)
                    {
                        var user = await _contextSql.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.Name == "Admin");
                        receiptModel.SupplierId = user.Id;
                    }


                    foreach (var detail in receiptModel.ReceiptDetails)
                    {
                        if (detail.ProductId > 0)
                        {
                            var oldProduct = await _contextSql.Products.FirstAsync(p => p.Id == detail.ProductId).ConfigureAwait(false);

                            productDetail = oldProduct;
                            productDetail.UpdateStock(detail.Quantity);
                            _contextSql.Products.Update(productDetail);
                        }

                        else { detail.ProductId = -1; }
                    }

                    
                    await _contextSql.Receipts.AddAsync(receiptModel, ct).ConfigureAwait(false);
                    
                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }


            return Ok(new IdResponse<long>(receiptModel.Id));
        }

        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, CancellationToken ct = default)
        {
            try
            {
                var model = await _contextSql
                                             .Receipts
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


    }
}
