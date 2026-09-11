
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.PDF
{
    public class PdfController : ApiBaseController
    {
        private readonly PdfService _service;
        private readonly EmailService _emailservice;


        public PdfController(PdfService service, EmailService emailservice)
        {
            _service = service;
            _emailservice = emailservice;
        }

        //COmprobante de Compra
        [HttpGet]
        [Route("PdfComprobanteVenta")]
        [AllowAnonymous]
        public async Task<IActionResult> PdfComprobanteVenta(long id, [FromServices] InvoiceService service)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            var factura = await service.GetById(id, requestedBy);
            var dtoEncabezado = new DtoRequestEncabezadoPDF()
            {
                TituloComprobante = "Factura Proforma",
                NumeroComprobante = factura.Data.InvoiceNumber.ToString()

            };

            var dtoCabecera = new DtoRequestCabeceraPDF()
            {
                Cuit = factura.Data.CustomerCuit,
                Direccion = factura.Data.CustomerAddress,
                Nombre = factura.Data.CustomerName,
                Observacion = factura.Data.Observation,
                Fecha = factura.Data.DateTime,
            };

            if (Enum.TryParse<ETypeReceipt>(factura.Data.Status, out var typeReceipt))
            {
                if(typeReceipt == ETypeReceipt.EXENTO)
                {
                    dtoCabecera.Tipo = "B";
                }
                else { dtoCabecera.Tipo = factura.Data.Status; }
            }

            var dtoDetalle = new DtoRequestDetallePDF()
            {
                Detalle = factura.Data.InvoiceDetails,
                Cantidad = factura.Data.InvoiceDetails.Select(p => p.Quantity).FirstOrDefault(),
                Producto = factura.Data.InvoiceDetails.Select(p => p.ProductName).FirstOrDefault(),
                Iva10 = (int)factura.Data.Iva10,
                Iva21 = (int)factura.Data.Iva21,
                Iva27 = (int)factura.Data.Iva27,
                Precio = (int)factura.Data.InvoiceDetails.Select(p => p.Price).FirstOrDefault(),
                IvaTotal = factura.Data.IvaTotal,
                Total = factura.Data.Total

            };

            Paragraph encabezado = await _service.Encabezado(dtoEncabezado);
            Paragraph Cabecera = await _service.Cabecera(dtoCabecera);
            Paragraph Detalle = await _service.Detalle(dtoDetalle);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(encabezado);
            paragraph.Add(Cabecera);
            paragraph.Add(Detalle);

            var contenido = await _service.Imprimir(paragraph).ConfigureAwait(false);
            return File(contenido.Data, "application/pdf", $"FacturaProforma_{DateTime.Now:dd-MM-yyyy}.pdf");
        } 

        //COmprobante de venta
        [HttpGet]
        [Route("PdfComprobanteCompra")]
        [AllowAnonymous]
        public async Task<IActionResult> PdfComprobanteCompra(long id, [FromServices] ReceiptService service)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            var factura = await service.GetById(id, requestedBy);
            var dtoEncabezado = new DtoRequestEncabezadoPDF()
            {
                TituloComprobante = "Comprobante De Compra",
                NumeroComprobante = factura.Data.ReceiptNumber.ToString()

            };

            var dtoCabecera = new DtoRequestCabeceraPDF()
            {
                Cuit = factura.Data.SupplierCuit,
                Direccion = factura.Data.SupplierAddress,
                Nombre = factura.Data.SupplierName,
                Fecha = factura.Data.DateTime,
                Observacion = factura.Data.Observation,
            };
            if (Enum.TryParse<ETypeReceipt>(factura.Data.Status, out var typeReceipt))
            {
                if (typeReceipt == ETypeReceipt.EXENTO)
                {
                    dtoCabecera.Tipo = "B";
                }
                else { dtoCabecera.Tipo = factura.Data.Status; }
            }

            var dtoDetalle = new DtoRequestDetallePDF()
            {
                ReceiptDetails = factura.Data.ReceiptDetails,
                Cantidad = factura.Data.ReceiptDetails.Select(p => p.Quantity).FirstOrDefault(),
                Producto = factura.Data.ReceiptDetails.Select(p => p.ProductName).FirstOrDefault(),
                ConcNoGravado = (int)factura.Data.ConcNoGravado,
                PercIva = (int)factura.Data.PercIva,
                PercIngBrutos = (int)factura.Data.PercIngBrutos,
                Precio = (int)factura.Data.ReceiptDetails.Select(p => p.Price).FirstOrDefault(),
                IvaTotal = (int)factura.Data.IvaTotal,
                Total = factura.Data.Total
            };

            Paragraph encabezado = await _service.Encabezado(dtoEncabezado);
            Paragraph Cabecera = await _service.Cabecera(dtoCabecera);
            Paragraph Detalle = await _service.DetalleComprobanteCompra(dtoDetalle);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(encabezado);
            paragraph.Add(Cabecera);
            paragraph.Add(Detalle);

           var contenido = await _service.Imprimir(paragraph);
          return File(contenido.Data, "application/pdf", $"ComprobanteVenta_{DateTime.Now:dd-MM-yyyy}.pdf");
        }

        //Presupuesto
        [HttpGet]
        [Route("PdfPresupuesto")]
        [AllowAnonymous]
        public async Task<IActionResult> PdfPresupuesto(long id, [FromServices] BudgetService service)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            var factura = await service.GetById(id, requestedBy);
            var dtoEncabezado = new DtoRequestEncabezadoPDF()
            {
                TituloComprobante = "Presupuesto",
                NumeroComprobante = factura.Data.BudgetNumber.ToString()
            };

            var dtoCabecera = new DtoRequestCabeceraPDF()
            {
              
                Direccion = factura.Data.CustomerAddress,
                Cuit = null,
                Nombre = factura.Data.CustomerName,
                Fecha = factura.Data.DateTime,
                Observacion = factura.Data.Observation
            };

            var dtoDetalle = new DtoRequestDetallePDF()
            {
                BudgetDetails = factura.Data.BudgetDetails,
                Cantidad = factura.Data.BudgetDetails.Select(p => p.Quantity).FirstOrDefault(),
                Producto = factura.Data.BudgetDetails.Select(p => p.ProductName).FirstOrDefault(),
                Precio = (decimal)factura.Data.BudgetDetails.Select(p => p.Price).FirstOrDefault(),
                Total = (decimal)factura.Data.Total
            };

            Paragraph encabezado = await _service.Encabezado(dtoEncabezado);
            Paragraph Cabecera = await _service.Cabecera(dtoCabecera);
            Paragraph Detalle = await _service.DetallePresupuestoYRemito(dtoDetalle);
            Paragraph Observacion = await _service.Observacion(dtoCabecera.Observacion);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(encabezado);
            paragraph.Add(Cabecera);
            paragraph.Add(Detalle);
            paragraph.Add(Observacion);
            var contenido = await _service.Imprimir(paragraph);
            return File(contenido.Data, "application/pdf", $"Presupuesto_{DateTime.Now:dd-MM-yyyy}.pdf");
        }

        [HttpGet]
        [Route("PdfRemito")]
        [AllowAnonymous]
        public async Task<IActionResult> PdfRemito(long id, [FromServices] DeliveryNotesService service)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            var factura = await service.GetById(id, requestedBy);
            var dtoEncabezado = new DtoRequestEncabezadoPDF()
            {
                TituloComprobante = "Remito",
                NumeroComprobante = factura.Data.DeliveryNotesNumber.ToString()
            };

            var dtoCabecera = new DtoRequestCabeceraPDF()
            {

                Direccion = factura.Data.SupplierAddress,
                Cuit = null,
                Nombre = factura.Data.SupplierName,
                Fecha = factura.Data.DateTime,
                Observacion = factura.Data.Observation
            };

            var dtoDetalle = new DtoRequestDetallePDF()
            {
                DeliveryNotesDetails = factura.Data.DeliveryNotesDetails,
                Cantidad = factura.Data.DeliveryNotesDetails.Select(p => p.Quantity).FirstOrDefault(),
                Producto = factura.Data.DeliveryNotesDetails.Select(p => p.ProductName).FirstOrDefault(),
                Precio = (decimal)factura.Data.DeliveryNotesDetails.Select(p => p.Price).FirstOrDefault(),
                Total = (decimal)factura.Data.ImportTotal,
                Iva = 0
            };

            Paragraph encabezado = await _service.Encabezado(dtoEncabezado);
            Paragraph Cabecera = await _service.Cabecera(dtoCabecera);
            Paragraph Detalle = await _service.DetallePresupuestoYRemito(dtoDetalle);
            Paragraph Observacion = await _service.Observacion(dtoCabecera.Observacion);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(encabezado);
            paragraph.Add(Cabecera);
            paragraph.Add(Detalle);
            paragraph.Add(Observacion);
            var contenido = await _service.Imprimir(paragraph);
            return File(contenido.Data, "application/pdf", $"Remito_{DateTime.Now:dd-MM-yyyy}.pdf");
        }

        [HttpGet]
        [Route("PdfRecibo")]
        [AllowAnonymous]
        public async Task<IActionResult> PdfRecibo(long id, [FromServices] QuittanceService service)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            var factura = await service.GetById(id, requestedBy);
            var dtoEncabezado = new DtoRequestEncabezadoPDF()
            {
                TituloComprobante = "Recibo",
                NumeroComprobante = factura.Data.QuittanceNumber.ToString()
            };

            var dtoCabecera = new DtoRequestCabeceraPDF()
            {

                Direccion = factura.Data.Address,
                Cuit = factura.Data.CustomerCuit,
                Nombre = factura.Data.CustomerName,
                Fecha = factura.Data.DateTime,
                Observacion = null
            };

            var dtoDetalle = new DtoRequestDetallePDF()
            {

                QuittanceProductDetails = factura.Data?.QuittanceProductDetails.ToList(),
                QuittanceDetails = factura.Data.QuittanceDetails,
                Banco = factura.Data.QuittanceDetails.Select(p => p.Bank).FirstOrDefault(),
                CheckNumber = factura.Data.QuittanceDetails.Select(p => p.CheckNumber).FirstOrDefault(),
                Concept = factura.Data.Concept,
                Cash = factura.Data.Cash,
                Total2 = factura.Data.Total
            };

            Paragraph encabezado = await _service.Encabezado(dtoEncabezado);
            Paragraph Cabecera = await _service.Cabecera(dtoCabecera);
            Paragraph Detalle = await _service.DetalleRecibo(dtoDetalle);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(encabezado);
            paragraph.Add(Cabecera);
            paragraph.Add(Detalle);
            var contenido = await _service.Imprimir(paragraph);
            return File(contenido.Data, "application/pdf", $"Recibo_{DateTime.Now:dd-MM-yyyy}.pdf");
        }

        [HttpGet]
        [Route("pdfcomprobanteventaarca")]
        [AllowAnonymous]
        public async Task<IActionResult> PdfComprobanteARCA(long id, [FromServices] InvoiceService invoiceService)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return File(await GenerarPdfFactura(id, invoiceService, requestedBy), "application/pdf", $"Factura_{DateTime.Now:dd-MM-yyyy}.pdf");
        }

        /// <summary>
        /// Send Invoice Attachment to client and generate PDF for the invoice.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invoiceService"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("enviarpdfcomprobanteventa")]
        [AllowAnonymous]
        public async Task<IActionResult> EnviarPdfComprobanteARCA(long id, string emailTo, [FromServices] InvoiceService invoiceService)
        { 
            if (string.IsNullOrEmpty(emailTo))
            {
                return BadRequest("Email address is required.");
            }
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _emailservice.SendEmailInvoice(emailTo, await GenerarPdfFactura(id, invoiceService, requestedBy)));
        }

        #region Private Method
        private async Task<byte[]> GenerarPdfFactura(long id, InvoiceService invoiceService, RequestedBy requestedBy)
        {
            var factura = await invoiceService.GetById(id, requestedBy);
            if (factura.Data == null) return null;

            var contenido = await _service.PrintInvoiceARCA(factura.Data);

            return contenido.Data;
        }

        #endregion
    }
}
