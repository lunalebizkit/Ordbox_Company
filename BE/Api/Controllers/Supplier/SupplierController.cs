using Ordbox.Services.Common;
using Ordbox.Services.Services;
using Ordbox.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Api.Controllers.Supplier
{
    public class SupplierController : ApiBaseController
    {
        private readonly EntityService _service;
        public SupplierController(EntityService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un Proveedor buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewSupplier })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            return Return(await _service.GetSupplierById(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Proveedores creados, con paginas y filtrado por nombre, dni y cuit.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewSupplier })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<string> filter)
        {
            return Return(await _service.ListSupplier(filter).ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega un Proveedor nuevo a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateSupplier })]
        public async Task<IActionResult> New([FromBody] DtoEntity model)
        {
            return Return(await _service.AddSupplier(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita un Proveedor ya creado y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditSupplier })]
        public async Task<IActionResult> Edit([FromBody] DtoEntity model)
        {
            return Return(await _service.UpdateSupplier(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un Proveedor buscando en la BASE DE DATOS por CUIT.
        /// </summary>
        /// <param name="cuit"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewSupplier })]
        [Route("[action]")]
        public async Task<IActionResult> GetSupplierByCuit([FromQuery] string cuit)
        {
            return Return(await _service.GetSupplierByCuit(cuit).ConfigureAwait(false));
        }
    }
}
