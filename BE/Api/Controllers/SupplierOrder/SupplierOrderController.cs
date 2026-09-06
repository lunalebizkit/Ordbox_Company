using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Ordbox.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;

namespace Ordbox.Api.Controllers.SupplierOrder
{
    public class SupplierOrderController : ApiBaseController
    {
        private readonly SupplierOrderService _service;
        public SupplierOrderController(SupplierOrderService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve una Orden de Pedido buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewOrderSupplier })]
        public async Task<IActionResult> GetById(long id)
        {
            return Return(await _service.GetById(id));
        }

        /// <summary>
        /// Agrega una nueva Orden de Pedido a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateOrderSupplier })]
        public async Task<IActionResult> New([FromBody] DtoRequestSupplierOrder model)
        {
            return Return(await _service.AddOrUpdate(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Crea una Orden de Pedido y en conjunto envia el email de pedido al proveedor.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateOrderSupplier })]
        [Route("orderAndEmail")]
        public async Task<IActionResult> NewWithEmail([FromBody] DtoRequestSupplierOrder model)
        {
            return Return(await _service.AddOrUpdate(model, true).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita una Orden de Pedido y la guarda modificada en la BASE DE DATOS, en conjunto con "email."
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditOrderSupplier })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestSupplierOrder model)
        {
            return Return(await _service.AddOrUpdate(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve el listado de Ordenes de Pedidos creadas, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewOrderSupplier })]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<ProductFilter> filter)
        {
            return Return(await _service.List(filter).ConfigureAwait(false));
        }

        /// <summary>
        /// Envia el email de Orden de Pedido.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateOrderSupplier })]
        [Route("email")]
        public async Task<IActionResult> SendOrderEmail(DtoSendOrderEmail model)
        {
            return Return(await _service.SendOrderEmail(model).ConfigureAwait(false));
        }
    }
}
