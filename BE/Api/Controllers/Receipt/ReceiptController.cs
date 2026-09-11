using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Receipt
{
    public class ReceiptController : ApiBaseController
    {
        private readonly ReceiptService _service;

        public ReceiptController(ReceiptService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un Orden de Compra buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetReceipt })]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega una nueva Orden de Compra a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateReceipt })]
        public async Task<IActionResult> New([FromBody] DtoRequestReceipt model )
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.NewReceipt(model, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Ordenes de Compra creadas, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetReceipt })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.ListReceipt(filter, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Inactiva una orden de compra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateReceipt })]
        public async Task<IActionResult> Delete(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Delete(id, requestedBy).ConfigureAwait(false));
        }
    }
}
