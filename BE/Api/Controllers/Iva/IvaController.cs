using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ordbox.Api.Controllers.Iva
{
    public class IvaController : ApiBaseController
    {
        private readonly IvaService _service;

        public IvaController(IvaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un listado del IVA de la compras realizadas en un periodo.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("listIvaCompra")]
        [AllowAccess(Permission = new EPermission[] { EPermission.ListIva })]
        public async Task<IActionResult> ListCompra([FromQuery]DateTime from, DateTime to)
        {
            return Return(await _service.ListIvaCompra(from, to).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado del IVA de la ventas realizadas en un periodo.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("listIvaVenta")]
        [AllowAccess(Permission = new EPermission[] { EPermission.ListIva })]
        public async Task<IActionResult> ListVenta([FromQuery] DateTime from, DateTime to)
        {
            return Return(await _service.ListIvaVenta(from, to).ConfigureAwait(false));
        }

        /// <summary>
        /// Descarga un excel con toda el listado de IVA compras.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReceiptIvaReport")]
        public async Task<IActionResult> ReceiptIvaReport([FromQuery] DateTime from, DateTime to)
        {
            var content = await _service.ReceiptIvaReport(from, to).ConfigureAwait(false);
            return File(content.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ListaReporteIvaCompra_{DateTime.Now:dd-MM-yyyy}.xlsx");
        }

        /// <summary>
        /// Descarga un excel con toda el listado de IVA ventas.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("InvoiceIvaReport")]
        public async Task<IActionResult> InvoiceIvaReport([FromQuery] DateTime from, DateTime to)
        {
            var content = await _service.InvoiceIvaReport(from, to).ConfigureAwait(false);
            return File(content.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ListaReporteIvaCompra_{DateTime.Now:dd-MM-yyyy}.xlsx");
        }

    }
}
