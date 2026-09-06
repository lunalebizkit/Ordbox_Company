using AutoMapper;
using Dapper;
using Ordbox.Domain;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.ARCA.Dto.Response;
using Ordbox.Services.Common;
using Ordbox.Services.ImpresoraFiscal;
using Ordbox.Services.ImpresoraFiscal.Printer250F;
using Ordbox.Services.LibroIvaDigital;
using Ordbox.Services.LibrosIvaDigital;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Scripts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Ordbox.Services.Services
{
    public class InvoiceService : BaseService
    {
        private readonly PrinterStatus _config;
        private readonly IPrinter _printer;

        public InvoiceService(ErrorManager logger, DBContext context, IMapper maper, IPrinter printer, PrinterStatus config, IConfiguration configuration) :
            base(logger, context, maper, configuration)
        {
            _config = config;
            _printer = printer;
        }
        public async Task<OperationResponse<DtoRequestInvoice>> GetById(long id)
        {
            try
            {
                var factura = await _contextSql
                                   .Invoices
                                   .Include(x => x.InvoiceDetails)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(p => p.Id == id)
                                   .ConfigureAwait(false);
                if (factura == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoRequestInvoice>(new OperationExceptions("000", $"Factura no encontrada {id}"));
                }

                var result = _mapper.Map<DtoRequestInvoice>(factura);

                result.Iva10 = 0;
                result.Iva21 = 0;
                result.Iva27 = 0;
                foreach (var item in result.InvoiceDetails)
                {
                    result.Iva10 += ((decimal)item.Iva == (decimal)10.5) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.105m : 0;
                    result.Iva21 += ((decimal)item.Iva == (decimal)21) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.21m : 0;
                    result.Iva27 += ((decimal)item.Iva == (decimal)27) ? (item.Quantity * item.Price) - (item.Quantity * item.Price) / 1.27m : 0;
                }
                return new OperationResponse<DtoRequestInvoice>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> NewInvoice(DtoRequestInvoice model, CancellationToken ct = default)
        {
            try
            {
                model.Id = 0;                
                return await AddOrUpdate(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<DtoPagination<DtoRequestListInvoice>>> ListInvoices(RequestPaginatedData<SpecificFilter> request, long companyId)
        {
            try
            {
                var query = _contextSql
                                    .Invoices
                                    .AsNoTracking()
                                    .Include(y => y.User).ThenInclude(x => x.Company)
                                    .Where(p => p.User.CompanyId == companyId && (!string.IsNullOrEmpty(request.Filter.Cuit) ? p.CustomerCuit.ToLower().Contains(request.Filter.Cuit) : true)
                                     && ((request.Filter.Number.HasValue && request.Filter.Number != 0) ? p.InvoiceNumber == request.Filter.Number : true)
                                     &&
                                     ((!request.Filter.Date.Contains("") || request.Filter.Date != null) ? p.DateTime.Date.ToString().Contains(request.Filter.Date) : true)
                                     &&
                                     (!string.IsNullOrEmpty(request.Filter.CustomerName) ? p.CustomerName.ToLower().Contains(request.Filter.CustomerName) : true)
                                     );

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderByDescending(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                var result = _mapper.Map<List<DtoRequestListInvoice>>(list);


                return new OperationResponse<DtoPagination<DtoRequestListInvoice>>(new DtoPagination<DtoRequestListInvoice>
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

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestInvoice model, CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            var invoiceModel = _mapper.Map<Invoice>(model);
            invoiceModel.DateTime = DateTime.Now;

            var newProduct = new Product();
            try
            {
                if (invoiceModel.Id == 0)
                {
                    var regex = new Regex(@"^-?[0-9][0-9,\.]+$");

                    #region VERIFICACIONES
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
                    if ((model.Type == (int)ETypeReceipt.A || model.Type == (int)ETypeReceipt.ResponsableMonotrinuto) && model.CustomerCuit.Length != 11)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, no puede cargar un DNI con Factura tipo A"));
                    }
                    //Verifico que el DNI tenga mayor a 7 caracteres y menor a 9
                    if (model.Type == (int)ETypeReceipt.B && model.CustomerCuit.Length < 7 || model.CustomerCuit.Length > 9 && model.CustomerCuit.Length != 11)
                    {
                        _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                        return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, verifique DNI"));
                    }
                    #endregion

                    if (invoiceModel.CustomerId == 0)
                    {
                        invoiceModel.CustomerId = GetUserAdminId();
                    }

                    foreach (var detail in invoiceModel.InvoiceDetails)
                    {
                        if (detail.ProductId > 0)
                        {
                            var oldProduct = await _contextSql.Products.FirstAsync(p => p.Id == detail.ProductId).ConfigureAwait(false);

                            newProduct = oldProduct;
                            newProduct.UpdateStock(-detail.Quantity);
                            _contextSql.Products.Update(newProduct);
                        }
                        if (detail.ProductId < 0)
                        {
                            detail.ProductId = -1;
                        }
                    }

                    if (_config.InvoiceStatus)
                    {
                        var error = await PrintInvoice(invoiceModel, ct);

                        #region ERRORES

                        if (error == "ErrorCliente")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cargar cliente, compruebe el CUIT/DNI"));
                        }

                        if (error == "ErrorAbrir")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al abrir documento , intente nuevamente"));
                        }

                        if (error == "ErrorImprimir")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al imprimir item, intente con un cierre Z"));
                        }

                        if (error == "ErrorCerrar")
                        {
                            _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                            return Error<IdResponse<long>>(new OperationExceptions("000", "Error al cerrar documento, intente con un cierre Z"));
                        }

                        #endregion

                        invoiceModel.InvoiceNumber = long.Parse(error);
                    }

                    await _contextSql.Invoices.AddAsync(invoiceModel, ct).ConfigureAwait(false);
                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                transaction.Commit();
                return Ok(new IdResponse<long>(invoiceModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO)));
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestInvoice model, DtoResponseARCAInvoice responseARCAInvoice,CancellationToken ct = default)
        {
            var transaction = _contextSql.Database.BeginTransaction();
            var invoiceModel = _mapper.Map<Invoice>(model);
            var newProduct = new Product();
            try
            {
                if (invoiceModel.Id != 0)
                {  
                    if (invoiceModel.CustomerId == 0)
                    {
                        invoiceModel.CustomerId = GetUserAdminId();
                    }      
                    
                    Invoice invoice = await _contextSql.Invoices.FirstAsync(p => p.Id == invoiceModel.Id).ConfigureAwait(false);

                    invoiceModel.CAE = string.IsNullOrEmpty(responseARCAInvoice.Cae) ? null : responseARCAInvoice.Cae;
                    invoiceModel.CAEExpirationDate = responseARCAInvoice.FechaVencimientoCae.HasValue ? responseARCAInvoice.FechaVencimientoCae.Value : null;
                    invoiceModel.IntegrationSuccess = !string.IsNullOrEmpty(responseARCAInvoice.Cae);
                    invoiceModel.InvoiceNumber = responseARCAInvoice.InvoiceNumber;

                    _contextSql.Entry(invoice).State = EntityState.Detached;
                    _contextSql.Invoices.Update(invoiceModel);
                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                transaction.Commit();
                return Ok(new IdResponse<long>(invoiceModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO)));
            }
        }

        public async Task<OperationResponse<IEnumerable<DtoResponseInvoiceReportTotals>>> InvoiceReport(RequestPaginatedData<StoredProcedureFilter> request)
        {
            var dateFromParameter = new SqlParameter("@dateFrom", request.Filter.DateFrom.HasValue ? (object)request.Filter.DateFrom.Value : (object)DBNull.Value);
            var dateToParameter = new SqlParameter("@dateTo", request.Filter.DateTo.HasValue ? (object)request.Filter.DateTo.Value : (object)DBNull.Value);
            var categoryParameter = new SqlParameter("@categoryId", (request.Filter.CategoryId == 0 || !request.Filter.CategoryId.HasValue) ? (object)DBNull.Value : request.Filter.CategoryId.Value);

            string invoiceSPname = StoredProcedure.INVOICEREPORTS;
            string invoiceRPTname = StoredProcedure.INVOICEREPORTSTOTAL;
            try
            {
                var invoices = await _contextSql.InvoiceSPReports
                .FromSqlRaw($"EXEC {invoiceSPname} @dateFrom, @dateTo, @categoryId", dateFromParameter, dateToParameter, categoryParameter)
                .ToListAsync();

                var invoiceReportTotal = await _contextSql.InvoiceSPReportTotals
                .FromSqlRaw($"EXEC {invoiceRPTname} @dateFrom, @dateTo, @categoryId", dateFromParameter, dateToParameter, categoryParameter)
                .ToListAsync();
                
                if ((invoices == null || invoiceReportTotal == null) || (!invoices.Any() || !invoiceReportTotal.Any()))
                {
                    return Error<IEnumerable<DtoResponseInvoiceReportTotals>>(new OperationExceptions("000", $"El reporte no encontro registros"));
                }
                List<DtoResponseInvoiceReportTotals> invoiceReportTotals = _mapper.Map<List<DtoResponseInvoiceReportTotals>>(invoiceReportTotal);
                List<DtoResponseInviocesReport> invoiceReport = _mapper.Map<List<DtoResponseInviocesReport>>(invoices);


                foreach (var item in invoiceReportTotals)
                {
                    item.InvoicesReports = new List<DtoResponseInviocesReport>();

                    item.InvoicesReports = invoiceReport.Where(y => y.Date.Date == item.InvoiceDate.Value).ToList();
                }

                return new OperationResponse<IEnumerable<DtoResponseInvoiceReportTotals>>(invoiceReportTotals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return Error<IEnumerable<DtoResponseInvoiceReportTotals>>(new OperationExceptions(ErrorsCodes.C_999_ERROR_GENERICO, ex?.Message.ToString()));

            }
        }

        public IEnumerable<Invoice> GetInvoiceByDate(DateTime from, DateTime to, CancellationToken ct = default)
        {
            try
            {
                IEnumerable<Invoice> invoice = new List<Invoice>();
                Dictionary<long, Invoice> invoiceDictionary = new Dictionary<long, Invoice>();
                string invoicesScript = SqlScripts.GetInvoiceByDate;

                using (var connection = new SqlConnection(ConnectionString))
                {
                    invoice = connection.Query<Invoice, InvoiceDetail, Invoice>
                        (sql: invoicesScript,
                            (invoice, detail) =>
                            {
                            if (!invoiceDictionary.TryGetValue(invoice.Id, out Invoice inv)) 
                                {
                                    inv = invoice;
                                    invoiceDictionary.Add(inv.Id, inv);
                                }
                                inv.InvoiceDetails.Add(detail);

                                return inv;
                            },
                        splitOn: "id",
                        param: new { @textdatefrom = from, @textdateto = to });
                }

                return invoiceDictionary.Values.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IEnumerable<DtoResponseIntegrationLogInvoice>>> GetIntegrationLogById(long invoiceId, CancellationToken ct = default)
        {
            try
            {
                IEnumerable<DtoResponseIntegrationLogInvoice> logs = new List<DtoResponseIntegrationLogInvoice>();
                string invoicesScript = SqlScripts.GetIntegrationLogInvoiceByInvoiceId;
                using (var connection = new SqlConnection(ConnectionString))
                {
                    logs = await connection.QueryAsync<DtoResponseIntegrationLogInvoice>
                        (invoicesScript, param: new { @invoiceid = invoiceId });
                }
                return new OperationResponse<IEnumerable<DtoResponseIntegrationLogInvoice>>(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex: ex);
                throw;
            }
        }

        #region Private Method
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
        private static string TruncateString(string value, int lenght)
        {
            return value.Substring(0, Math.Min(value.Length, lenght));
        }

        #endregion

        #region Alicuota Digital

        public async Task<OperationResponse<byte[]>> AlicuotaTxt(IEnumerable<Invoice> invoices, CancellationToken ct = default)
        {

            StringWriter OutPutFile = new StringWriter();
            List<AlicuotaIvaDto> alicuotaIvaDtos = new List<AlicuotaIvaDto>();
            try
            {

                MemoryStream ms = new MemoryStream();
                TextWriter tw = new StreamWriter(ms, Encoding.GetEncoding("ISO-8859-1"));

                foreach (Invoice item in invoices)
                {
                    bool tieneIva10 = item.InvoiceDetails.Any(d => d.Iva == 10.5m);
                    bool tieneIva21 = item.InvoiceDetails.Any(d => d.Iva == 21m);
                    bool tieneIva27 = item.InvoiceDetails.Any(d => d.Iva == 27m);

                    decimal totalIva10 = 0;
                    decimal totalBaseIva10 = 0;
                    decimal totalIva21 = 0;
                    decimal totalBaseIva21 = 0;
                    decimal totalIva27 = 0;                    
                    decimal totalBaseIva27 = 0;

                    // Variables para facturas B con iva incluido
                    decimal totalBaseIva10B = 0;
                    decimal totalBaseIva21B = 0;             
                    decimal totalBaseIva27B = 0;                    

                    foreach (InvoiceDetail invoiceDetail in item.InvoiceDetails)
                    {
                        if (item.Type == (int)ETypeReceipt.A || item.Type == (int)ETypeReceipt.ResponsableMonotrinuto)
                        {
                            #region Importe Liquidado (total de iva)
                            totalIva10 += ((decimal)invoiceDetail.Iva == (decimal)10.5) ? (invoiceDetail.Quantity * invoiceDetail.Price) - (invoiceDetail.Quantity * invoiceDetail.Price) / 1.105m : 0;
                            totalBaseIva10 += ((decimal)invoiceDetail.Iva == (decimal)10.5) ? (invoiceDetail.Quantity * invoiceDetail.Price) : 0;

                            totalIva21 += ((decimal)invoiceDetail.Iva == (decimal)21) ? (invoiceDetail.Quantity * invoiceDetail.Price) - (invoiceDetail.Quantity * invoiceDetail.Price) / 1.21m : 0;
                            totalBaseIva21 += ((decimal)invoiceDetail.Iva == (decimal)21) ? (invoiceDetail.Quantity * invoiceDetail.Price) : 0;

                            totalIva27 += ((decimal)invoiceDetail.Iva == (decimal)27) ? (invoiceDetail.Quantity * invoiceDetail.Price) - (invoiceDetail.Quantity * invoiceDetail.Price) / 1.27m : 0;
                            totalBaseIva27 += ((decimal)invoiceDetail.Iva == (decimal)27) ? (invoiceDetail.Quantity * invoiceDetail.Price) : 0;
                            #endregion
                        }
                        if (item.Type == (int)ETypeReceipt.B || item.Type == (int)ETypeReceipt.EXENTO)
                        {
                            totalBaseIva10B += ((decimal)invoiceDetail.Iva == (decimal)10.5) ? (invoiceDetail.Quantity * invoiceDetail.Price) : 0;

                            totalBaseIva21B += ((decimal)invoiceDetail.Iva == (decimal)21) ? (invoiceDetail.Quantity * invoiceDetail.Price) : 0;

                            totalBaseIva27B += ((decimal)invoiceDetail.Iva == (decimal)27) ? (invoiceDetail.Quantity * invoiceDetail.Price) : 0;
                        }
                     }
                    
                    if (tieneIva10) 
                    {

                        AlicuotaIvaDto alicuotaIva = new AlicuotaIvaDto();

                        #region Condicionales Tipo Factura
                        if (item.Type == (int)ETypeReceipt.B || item.Type == (int)ETypeReceipt.EXENTO)
                        {
                            alicuotaIva.TipoDecComprobante = CustomizationConstant.FacturaB;

                            #region Importe neto gravado (SIN COMA)
                            var netogravado = Math.Round((totalBaseIva10B / 1.105m), 2);
                            alicuotaIva.ImporteNetoGravado = netogravado.ToString().Replace(",", "").Replace(".", "");
                            alicuotaIva.ImporteNetoGravado = alicuotaIva.ImporteNetoGravado.PadLeft(15, '0');
                            #endregion

                            #region Impuesto Liquidado
                            var impuestoLiquidado = Math.Round((netogravado * 0.105m),2);
                            alicuotaIva.ImpuestoLiquidado = impuestoLiquidado.ToString("F2").Replace(",", "").Replace(".", "");
                            alicuotaIva.ImpuestoLiquidado = alicuotaIva.ImpuestoLiquidado.PadLeft(15, '0');
                            #endregion
                        }

                        if (item.Type == (int)ETypeReceipt.A || item.Type == (int)ETypeReceipt.ResponsableMonotrinuto)
                        {
                            alicuotaIva.TipoDecComprobante = CustomizationConstant.FacturaA;

                            #region Importe neto gravado (SIN COMA)
                            var subtotal = totalBaseIva10 - Math.Round(totalIva10, 2);
                            alicuotaIva.ImporteNetoGravado = subtotal.ToString().Replace(",", "").Replace(".", "");
                            alicuotaIva.ImporteNetoGravado = alicuotaIva.ImporteNetoGravado.PadLeft(15, '0');
                            #endregion

                            #region Impuesto Liquidado
                            alicuotaIva.ImpuestoLiquidado = totalIva10.ToString("F2").Replace(",", "").Replace(".", "");
                            alicuotaIva.ImpuestoLiquidado = alicuotaIva.ImpuestoLiquidado.PadLeft(15, '0');
                            #endregion
                        }
                        #endregion

                        #region Numero de Comprobante
                        alicuotaIva.NumeroDeComprobante = item.InvoiceNumber.ToString().PadLeft(20, '0');
                        #endregion
                                                
                        #region Condicionales Iva
                        
                        alicuotaIva.AlicuotaIva = "4";

                        #endregion

                        alicuotaIvaDtos.Add(alicuotaIva);
                    }
                    
                    if (tieneIva21) 
                    {
                        AlicuotaIvaDto alicuotaIva = new AlicuotaIvaDto();

                        #region Condicionales Tipo Factura
                        if (item.Type == (int)ETypeReceipt.B || item.Type == (int)ETypeReceipt.EXENTO)
                        {
                            alicuotaIva.TipoDecComprobante = CustomizationConstant.FacturaB;

                            #region Importe neto gravado (SIN COMA)
                            var netogravado = Math.Round((totalBaseIva21B / 1.21m), 2);
                            alicuotaIva.ImporteNetoGravado = netogravado.ToString().Replace(",", "").Replace(".", "");
                            alicuotaIva.ImporteNetoGravado = alicuotaIva.ImporteNetoGravado.PadLeft(15, '0');
                            #endregion

                            #region Impuesto Liquidado
                            var impuestoLiquidado = Math.Round((netogravado * 0.21m), 2);
                            alicuotaIva.ImpuestoLiquidado = impuestoLiquidado.ToString("F2").Replace(",", "").Replace(".", "");
                            alicuotaIva.ImpuestoLiquidado = alicuotaIva.ImpuestoLiquidado.PadLeft(15, '0');
                            #endregion
                        }

                        if (item.Type == (int)ETypeReceipt.A || item.Type == (int)ETypeReceipt.ResponsableMonotrinuto)
                        {
                            alicuotaIva.TipoDecComprobante = CustomizationConstant.FacturaA;

                            #region Importe neto gravado (SIN COMA)
                            var subtotal = totalBaseIva21 - Math.Round(totalIva21, 2);
                            alicuotaIva.ImporteNetoGravado = subtotal.ToString().Replace(",", "").Replace(".", "");
                            alicuotaIva.ImporteNetoGravado = alicuotaIva.ImporteNetoGravado.PadLeft(15, '0');
                            #endregion

                            #region Impuesto Liquidado
                            alicuotaIva.ImpuestoLiquidado = totalIva21.ToString("F2").Replace(",", "").Replace(".", "");
                            alicuotaIva.ImpuestoLiquidado = alicuotaIva.ImpuestoLiquidado.PadLeft(15, '0');
                            #endregion
                        }
                        #endregion

                        #region Numero de Comprobante
                        alicuotaIva.NumeroDeComprobante = item.InvoiceNumber.ToString().PadLeft(20, '0');
                        #endregion
                        
                        #region Condicionales Iva                        
                        alicuotaIva.AlicuotaIva = "5";                        
                        #endregion

                        alicuotaIvaDtos.Add(alicuotaIva);
                    }
                    
                    if (tieneIva27) 
                    {
                        AlicuotaIvaDto alicuotaIva = new AlicuotaIvaDto();

                        #region Condicionales Tipo Factura
                        if (item.Type == (int)ETypeReceipt.B || item.Type == (int)ETypeReceipt.EXENTO)
                        {
                            alicuotaIva.TipoDecComprobante = CustomizationConstant.FacturaB;

                            #region Importe neto gravado (SIN COMA)
                            var netogravado = Math.Round((totalBaseIva27B / 1.27m), 2);
                            alicuotaIva.ImporteNetoGravado = netogravado.ToString().Replace(",", "").Replace(".", "");
                            alicuotaIva.ImporteNetoGravado = alicuotaIva.ImporteNetoGravado.PadLeft(15, '0');
                            #endregion

                            #region Impuesto Liquidado
                            var impuestoLiquidado = Math.Round((netogravado * 0.27m), 2);
                            alicuotaIva.ImpuestoLiquidado = impuestoLiquidado.ToString("F2").Replace(",", "").Replace(".", "");
                            alicuotaIva.ImpuestoLiquidado = alicuotaIva.ImpuestoLiquidado.PadLeft(15, '0');
                            #endregion

                        }

                        if (item.Type == (int)ETypeReceipt.A || item.Type == (int)ETypeReceipt.ResponsableMonotrinuto)
                        {
                            alicuotaIva.TipoDecComprobante = CustomizationConstant.FacturaA;

                            #region Importe neto gravado (SIN COMA)
                            var subtotal = totalBaseIva27 - Math.Round(totalIva27, 2);
                            alicuotaIva.ImporteNetoGravado = subtotal.ToString().Replace(",", "").Replace(".", "");
                            alicuotaIva.ImporteNetoGravado = alicuotaIva.ImporteNetoGravado.PadLeft(15, '0');
                            #endregion

                            #region Impuesto Liquidado
                            alicuotaIva.ImpuestoLiquidado = totalIva27.ToString("F2").Replace(",", "").Replace(".", "");
                            alicuotaIva.ImpuestoLiquidado = alicuotaIva.ImpuestoLiquidado.PadLeft(15, '0');
                            #endregion
                        }
                        #endregion

                        #region Numero de Comprobante
                        alicuotaIva.NumeroDeComprobante = item.InvoiceNumber.ToString().PadLeft(20, '0');
                        #endregion

                        
                        #region Condicionales Iva                        
                        alicuotaIva.AlicuotaIva = "6";                        
                        #endregion

                        alicuotaIvaDtos.Add(alicuotaIva);
                    }
                }

                foreach (AlicuotaIvaDto alicuotaIvaDto in alicuotaIvaDtos)
                {
                    await tw.WriteAsync
                        (
                            alicuotaIvaDto.TipoDecComprobante +
                            alicuotaIvaDto.PuntoDeVenta.ToString().PadLeft(5, '0') +
                            alicuotaIvaDto.NumeroDeComprobante +
                            alicuotaIvaDto.ImporteNetoGravado +
                            alicuotaIvaDto.AlicuotaIva.PadLeft(4, '0') +
                            alicuotaIvaDto.ImpuestoLiquidado +
                            "\n"
                        );
                }

                tw.Flush();

                byte[] bytes = ms.ToArray();

                ms.Close();

                return new OperationResponse<byte[]>(bytes);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                OutPutFile.Close();
                OutPutFile.Dispose();
            }
        }

        #endregion  


        #region IvaDigital
        public async Task<OperationResponse<byte[]>> ArchivoTxt(IEnumerable<Invoice> invoices, CancellationToken ct = default)
        { 
            StringWriter OutPutFile = new StringWriter();

            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    TextWriter tw = new StreamWriter(ms, Encoding.GetEncoding("ISO-8859-1"));

                    List<string> archivoTxtDtos = new List<string>();

                    foreach (Invoice invoice in invoices)
                    {
                        ArchivoTxtDto archivoTxtDto = new ArchivoTxtDto();
                        string newIvaLine = string.Empty;

                        string nombreCompletoComprador = invoice.CustomerName.ToUpper().Trim();
                        string importeTotal = invoice.Total.ToString().Replace(",", "").Replace(".", "");

                        archivoTxtDto.FechaDeComprobante = TruncateString(invoice.DateTime.ToString("yyyyMMdd"), 8);

                        if (invoice.Type == (int)ETypeReceipt.B || invoice.Type == (int)ETypeReceipt.EXENTO)
                        {
                            archivoTxtDto.TipoDeComprobante = CustomizationConstant.FacturaB;
                        }

                        if (invoice.Type == (int)ETypeReceipt.A || invoice.Type == (int)ETypeReceipt.ResponsableMonotrinuto)
                        {
                            archivoTxtDto.TipoDeComprobante = CustomizationConstant.FacturaA;
                        }

                        archivoTxtDto.NumeroDeComprobante = TruncateString(invoice.InvoiceNumber.ToString().PadLeft(20, '0'), 20);
                        archivoTxtDto.NumeroDeComprobanteHasta = TruncateString(invoice.InvoiceNumber.ToString().PadLeft(20, '0'), 20);

                        if (invoice.CustomerCuit.Length == 8)
                        {
                            archivoTxtDto.CodigoDocumento = CustomizationConstant.DniId;
                        }

                        if (invoice.CustomerCuit.Length == 11)
                        {
                            if (invoice.CustomerCuit == CustomizationConstant.DefaultCUIT)
                            {
                                archivoTxtDto.CodigoDocumento = CustomizationConstant.NoCuitId;
                            }
                            else
                            {
                                archivoTxtDto.CodigoDocumento = CustomizationConstant.CuitId;
                            }
                        }

                        var cantidadAlicuota = invoice.InvoiceDetails.Select(y => y.Iva).Distinct().Count();
                        archivoTxtDto.AlicuotaIva = cantidadAlicuota.ToString();

                        archivoTxtDto.NumeroDeIdentificacionComprador = invoice.CustomerCuit == CustomizationConstant.DefaultCUIT ? TruncateString(CustomizationConstant.DefaultNoCUIT.PadLeft(20, '0'), 20) : TruncateString(invoice.CustomerCuit.Trim().ToString().PadLeft(20, '0'), 20);
                        archivoTxtDto.NombreCompletoComprador = TruncateString(nombreCompletoComprador.PadRight(30, ' '), 30);

                        archivoTxtDto.ImporteTotal = TruncateString(importeTotal.PadLeft(15, '0'), 15);

                        archivoTxtDto.FechaDePago = TruncateString(invoice.DateTime.ToString("yyyyMMdd"), 8);

                        newIvaLine = $"{archivoTxtDto.FechaDeComprobante}{archivoTxtDto.TipoDeComprobante}{archivoTxtDto.PuntoDeVenta}{archivoTxtDto.NumeroDeComprobante}{archivoTxtDto.NumeroDeComprobanteHasta}{archivoTxtDto.CodigoDocumento}{archivoTxtDto.NumeroDeIdentificacionComprador}{archivoTxtDto.NombreCompletoComprador}{archivoTxtDto.ImporteTotal}{archivoTxtDto.NetoGravado}{archivoTxtDto.NoCategorizados}{archivoTxtDto.OperacionesExentas}{archivoTxtDto.ImpuestosNacionales}{archivoTxtDto.IngresosBrutos}{archivoTxtDto.ImpuestosMunicipales}{archivoTxtDto.ImpuestosInternos}{archivoTxtDto.CodigoDeMoneda}{archivoTxtDto.TipoDeCambio}{archivoTxtDto.AlicuotaIva}{archivoTxtDto.CodigoDeOperacion}{archivoTxtDto.OtrosTributos}{archivoTxtDto.FechaDePago}";

                        archivoTxtDtos.Add(TruncateString(newIvaLine.Trim(), 266));
                    }

                    foreach (string item in archivoTxtDtos)
                    {
                        await tw.WriteAsync(item + "\r\n");
                    }

                    tw.Flush();

                    byte[] bytes = ms.ToArray();

                    ms.Close();

                    return new OperationResponse<byte[]>(bytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    OutPutFile.Close();
                    OutPutFile.Dispose();
                }
            }
        }
        #endregion 


        #region Imprimir Factura En impresora Fiscal
        public async Task<string> PrintInvoice(Invoice model, CancellationToken ct = default)
        {
            string? closeFactura = null;
            //MANEJO DE ERRORES
            var cargarCliente = await _printer.CargarDatosCliente(model.CustomerName, model.CustomerCuit, model.CustomerAddress, (ETypeReceipt)model.Type).ConfigureAwait(false);

            if (cargarCliente == null)
            {
                await _printer.CerrarJornadaFiscal();
                return "ErrorCliente";
            }
            //Contiene loop de reintentos en consultar Estado
            var openDoc = await _printer.OpenInvoice((ETypeReceipt)model.Type, model.CustomerName, eTypeDocumentClient.Cuil, model.CustomerAddress).ConfigureAwait(false);

            if (openDoc == null)
            {
                await _printer.CloseFactura(1, "").ConfigureAwait(false);
                return "ErrorAbrir";                
            }

            //TODO por cada item mandar a imprimir
            foreach (var item in model.InvoiceDetails)
            {
                //Contiene loop de reintentos en consultar Estado
                var imprimir = await _printer.PrintItem(item.ProductName, item.Quantity, item.Price, item.Iva, item.ProductCode.ToString()).ConfigureAwait(false);

                if (imprimir == null)
                {
                    //Intento recuperar numero de comprobante mediante Status
                    closeFactura = await _printer.CloseFactura(1, "", true).ConfigureAwait(false);

                    if (closeFactura == null)
                    {
                        return "ErrorImprimir";
                    }
                }
            }
            if (string.IsNullOrEmpty(closeFactura))
            {
                closeFactura = await _printer.CloseFactura(1, "").ConfigureAwait(false);
            }

            if (closeFactura == null)
            {
                Thread.Sleep(1000);
                await _printer.CerrarJornadaFiscal();
                return "ErrorCerrar";
            }

            return closeFactura;

        }

        #endregion

        #region Private
               
        #endregion
    }
}
