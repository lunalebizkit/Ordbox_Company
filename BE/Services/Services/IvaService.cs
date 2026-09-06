using AutoMapper;
using ClosedXML.Excel;
using Ordbox.Domain;
using Ordbox.Domain.Enum;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class IvaService : BaseService
    {
        public IvaService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
            base(logger, context, maper, configuration)
        { }

        public async Task<OperationResponse<DtoResponseIvaInvoice>> ListIvaVenta(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var query = _contextSql
                                    .Invoices.OrderBy(p => p.DateTime)
                                    .Include(p => p.InvoiceDetails)
                                    .Where(x => x.DateTime.Date >= from && x.DateTime.Date <= to)
                                    .AsNoTracking();

            var newDtoDetalleResumem = new List<DtoResponseIvaInvoices>();

            var resumen = new DtoResponseIvaInvoice();

            foreach (var item in query)
            {
                var newItem = _mapper.Map<DtoResponseIvaInvoices>(item);


                newItem.Iva10 = 0;
                newItem.Iva21 = 0;
                newItem.Iva27 = 0;

                resumen.PeriodTotal += newItem.Total;
                newItem.ImporteNeto += newItem.Total - newItem.IvaTotal;
                foreach (var detalle in item.InvoiceDetails)
                {
                    newItem.Iva10 += ((decimal)detalle.Iva == (decimal)10.5) ? (detalle.Quantity * detalle.Price) - (detalle.Quantity * detalle.Price) / 1.105m : 0;
                    newItem.Iva21 += ((decimal)detalle.Iva == (decimal)21) ? (detalle.Quantity * detalle.Price) - (detalle.Quantity * detalle.Price) / 1.21m : 0;
                    newItem.Iva27 += ((decimal)detalle.Iva == (decimal)27) ? (detalle.Quantity * detalle.Price) - (detalle.Quantity * detalle.Price) / 1.27m : 0;

                    newItem.ImporteNetoIva10 += ((decimal)detalle.Iva == (decimal)10.5) ? ((detalle.Quantity * detalle.Price) / 1.105m ): 0;
                    newItem.ImporteNetoIva21 += ((decimal)detalle.Iva == (decimal)21) ? ((detalle.Quantity * detalle.Price) / 1.21m): 0;
                    newItem.ImporteNetoIva27 += ((decimal)detalle.Iva == (decimal)27) ? ((detalle.Quantity * detalle.Price) / 1.27m ): 0;
                }


                newDtoDetalleResumem.Add(newItem);
            }
            resumen.DtoResponseIvaInvoices = newDtoDetalleResumem;

            return new OperationResponse<DtoResponseIvaInvoice>(resumen);

        }
        public async Task<OperationResponse<DtoResponseIvaReceipt>> ListIvaCompra(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var query = _contextSql
                                    .Receipts.OrderBy(p => p.DateTime).OrderBy(p => p.Type)
                                    .Include(p => p.ReceiptDetails)
                                    .Where(x => x.DateTime.Date >= from && x.DateTime.Date <= to)
                                    .AsNoTracking();

            var newDtoDetalleResumem = new List<DtoResponseIvaReceipts>();

            var resumen = new DtoResponseIvaReceipt();

            foreach (var item in query)
            {
                var newItem = _mapper.Map<DtoResponseIvaReceipts>(item);

                resumen.PeriodTotal += newItem.Total;
                newItem.ImporteNeto += newItem.Total - newItem.IvaTotal;
                foreach (var item2 in item.ReceiptDetails)
                {
                    newItem.Iva10 += ((decimal)item2.Iva == (decimal)10.5) ? (item2.Quantity * item2.Price * 10.5m) / 100.0m : 0;
                    newItem.Iva21 += ((decimal)item2.Iva == (decimal)21) ? (item2.Quantity * item2.Price * 21.0m) / 100.0m : 0;
                    newItem.Iva27 += ((decimal)item2.Iva == (decimal)27) ? (item2.Quantity * item2.Price * 27.0m) / 100.0m : 0;

                    newItem.ImporteNetoIva10 += ((decimal)item2.Iva == (decimal)10.5) ? ((item2.Quantity * item2.Price) / 1.105m) : 0;
                    newItem.ImporteNetoIva21 += ((decimal)item2.Iva == (decimal)21) ? ((item2.Quantity * item2.Price) / 1.21m) : 0;
                    newItem.ImporteNetoIva27 += ((decimal)item2.Iva == (decimal)27) ? ((item2.Quantity * item2.Price) / 1.27m) : 0;
                }


                newDtoDetalleResumem.Add(newItem);
            }
            resumen.DtoResponseIvaReceipts = newDtoDetalleResumem;

            return new OperationResponse<DtoResponseIvaReceipt>(resumen);

        }
        public async Task<OperationResponse<byte[]>> ReceiptIvaReport(DateTime from, DateTime to, CancellationToken ct = default)
        {
            MemoryStream stream= default;
            try
            {
                var query = await _contextSql
                                .Receipts.OrderBy(p => p.DateTime.Date).OrderBy(p => p.Type)
                                .Include(s => s.ReceiptDetails)
                                .AsNoTracking()
                                .Where(x => x.DateTime.Date >= from && x.DateTime.Date <= to).ToArrayAsync();

                var newDtoDetalleResumem = new List<DtoResponseIvaReceipts>();

                var resumen = new DtoResponseIvaReceipt();

                foreach (var item in query)
                {
                    var newItem = _mapper.Map<DtoResponseIvaReceipts>(item);

                    resumen.PeriodTotal += newItem.Total;
                    newItem.ImporteNeto += newItem.Total - newItem.IvaTotal;
                    foreach (var item2 in item.ReceiptDetails)
                    {
                        newItem.Iva10 += ((decimal)item2.Iva == (decimal)10.5) ? (item2.Quantity * item2.Price * 10.5m) / 100.0m : 0;
                        newItem.Iva21 += ((decimal)item2.Iva == (decimal)21) ? (item2.Quantity * item2.Price * 21.0m) / 100.0m : 0;
                        newItem.Iva27 += ((decimal)item2.Iva == (decimal)27) ? (item2.Quantity * item2.Price * 27.0m) / 100.0m : 0;

                        newItem.ImporteNetoIva10 += ((decimal)item2.Iva == (decimal)10.5) ? ((item2.Quantity * item2.Price) / 1.105m) : 0;
                        newItem.ImporteNetoIva21 += ((decimal)item2.Iva == (decimal)21) ? ((item2.Quantity * item2.Price) / 1.21m) : 0;
                        newItem.ImporteNetoIva27 += ((decimal)item2.Iva == (decimal)27) ? ((item2.Quantity * item2.Price) / 1.27m) : 0;
                    }

                    newDtoDetalleResumem.Add(newItem);
                }
                resumen.DtoResponseIvaReceipts = newDtoDetalleResumem;

                var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("Reporte Iva Compra");
                var currentRow = 2;
                var ColorHeader = XLColor.FromName("PowderBlue");

                worksheet.Style.Font.SetFontName("Arial");

                #region Header Columnas       

                worksheet.Cell(currentRow, 1).SetValue("Fecha").Style.DateFormat.Format = "mm/dd/YYYY";
                worksheet.Cell(currentRow, 1).Style.Font.Bold= true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 2).SetValue(" N° Factura").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 3).SetValue("Tipo").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 4).SetValue("Cliente").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 5).SetValue("CUIT / CUIL").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 6).SetValue("Imp.Neto").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 7).SetValue("IVA 10,5 %").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 8).SetValue("IVA 21 % ").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 8).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 9).SetValue("IVA 27 %").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 10).SetValue("Total").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 10).Style.Fill.BackgroundColor = ColorHeader;
                #endregion

                #region Body
                foreach (var item in resumen.DtoResponseIvaReceipts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).SetValue(item.DateTime.ToString("MM/dd/yyyy")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 2).SetValue(item.ReceiptNumber).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 3).SetValue(item.Type).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 4).SetValue(item.SupplierName).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 5).SetValue(item.SupplierCuit).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 6).SetValue(item.ImporteNeto).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 7).SetValue(item.Iva10 == 0 ? null : string.Format("{0,7:##.00}", item.Iva10)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 8).SetValue(item.Iva21 == 0 ? null : string.Format("{0,7:##.00}", item.Iva21)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 9).SetValue(item.Iva27 == 0 ? null : string.Format("{0,7:##.00}", item.Iva27)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 10).SetValue(item.Total).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right); ;
                }
                #endregion
                currentRow += 2;
                worksheet.Cell(currentRow, 9).SetValue("Total Periodo").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 10).SetValue("$ " + $"{resumen.PeriodTotal}").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                stream = new MemoryStream();

                workbook.SaveAs(stream);

                workbook.Dispose();

                return new OperationResponse<byte[]>(stream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
            finally
            {
                if (stream != default)
                {
                    stream.Dispose();
                }
            }
            
        }
        public async Task<OperationResponse<byte[]>> InvoiceIvaReport(DateTime from, DateTime to, CancellationToken ct = default)
        {
            MemoryStream stream = default;
            try
            {
                var query = await _contextSql
                                .Invoices.OrderBy(p => p.Type).OrderBy(p => p.DateTime.Date)
                                .Include(s => s.InvoiceDetails)
                                .AsNoTracking()
                                .Where(x => x.DateTime.Date >= from && x.DateTime.Date <= to).ToArrayAsync();

                var newDtoDetalleResumem = new List<DtoResponseIvaInvoices>();

                var resumen = new DtoResponseIvaInvoice();

                foreach (var item in query)
                {
                    var newItem = _mapper.Map<DtoResponseIvaInvoices>(item);

                    resumen.PeriodTotal += newItem.Total;
                    newItem.ImporteNeto += newItem.Total - newItem.IvaTotal;
                    foreach (var item2 in item.InvoiceDetails)
                    {
                        newItem.Iva10 += ((decimal)item2.Iva == (decimal)10.5) ? (item2.Quantity * item2.Price * 10.5m) / 100.0m : 0;
                        newItem.Iva21 += ((decimal)item2.Iva == (decimal)21) ? (item2.Quantity * item2.Price * 21.0m) / 100.0m : 0;
                        newItem.Iva27 += ((decimal)item2.Iva == (decimal)27) ? (item2.Quantity * item2.Price * 27.0m) / 100.0m : 0;

                        newItem.ImporteNetoIva10 += ((decimal)item2.Iva == (decimal)10.5) ? ((item2.Price * item2.Quantity) - (item2.Quantity * item2.Price * 10.5m) / 100.0m) : 0;
                        newItem.ImporteNetoIva21 += ((decimal)item2.Iva == (decimal)21) ? ((item2.Price * item2.Quantity) - (item2.Quantity * item2.Price * 21.0m) / 100.0m) : 0;
                        newItem.ImporteNetoIva27 += ((decimal)item2.Iva == (decimal)27) ? ((item2.Price * item2.Quantity) - (item2.Quantity * item2.Price * 27.0m) / 100.0m) : 0;
                    }


                    newDtoDetalleResumem.Add(newItem);
                }
                resumen.DtoResponseIvaInvoices = newDtoDetalleResumem;

                var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("Reporte Iva Venta");
                var currentRow = 2;
                var ColorHeader = XLColor.FromName("PowderBlue");

                worksheet.Style.Font.SetFontName("Arial");
                #region Header Columnas       

                worksheet.Cell(currentRow, 1).SetValue("Fecha").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 2).SetValue(" N° Factura").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 3).SetValue("Tipo").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 4).SetValue("Cliente").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 5).SetValue("CUIT / CUIL").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 6).SetValue("Imp.Neto").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 7).SetValue("IVA 10,5 %").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 8).SetValue("IVA 21 % ").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 8).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 9).SetValue("IVA 27 %").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 10).SetValue("Total").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 10).Style.Fill.BackgroundColor = ColorHeader;
                #endregion

                #region Body
                foreach (var item in resumen.DtoResponseIvaInvoices)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).SetValue(item.DateTime.ToString("MM/dd/yyyy")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 2).SetValue(item.InvoiceNumber).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 3).SetValue(item.Type).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 4).SetValue(item.CustomerName).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 5).SetValue(item.CustomerCuit).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Cell(currentRow, 6).SetValue(item.ImporteNeto).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 7).SetValue(item.Iva10 == 0 ? null : string.Format("{0,7:##.00}", item.Iva10)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 8).SetValue(item.Iva21 == 0 ? null : string.Format("{0,7:##.00}", item.Iva21)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 9).SetValue(item.Iva27 == 0 ? null : string.Format("{0,7:##.00}", item.Iva27)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 10).SetValue(item.Total).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right); ;
                }
                #endregion
                currentRow += 2;
                worksheet.Cell(currentRow, 9).SetValue("Total Periodo").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = ColorHeader;

                worksheet.Cell(currentRow, 10).SetValue("$ " + $"{resumen.PeriodTotal}").Style.Font.Bold = true;
                worksheet.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                
                stream = new MemoryStream();

                workbook.SaveAs(stream);

                workbook.Dispose();

                return new OperationResponse<byte[]>(stream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
            finally
            {
                if (stream != default)
                {
                    stream.Dispose();
                }
            }

        }
    }
}
