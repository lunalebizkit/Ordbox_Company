using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;

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
            return Return(await _service.GetById(id).ConfigureAwait(false));
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
            return Return(await _service.NewReceipt(model).ConfigureAwait(false));
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
            return Return(await _service.ListReceipt(filter, User.GetCompanyId()).ConfigureAwait(false));
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
            return Return(await _service.Delete(id).ConfigureAwait(false));
        }
    }
}
