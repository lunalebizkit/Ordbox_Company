using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Customer
{
    public class CustomerController : ApiBaseController
    {
        private readonly EntityService _service;
        public CustomerController(EntityService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un Cliente, buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCustomer })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un Cliente, buscando en la BASE DE DATOS por CUIT.
        /// </summary>
        /// <param name="cuit"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCustomer })]
        [Route("[action]")]
        public async Task<IActionResult> GetCustomerByCuit([FromQuery] string cuit)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetCustomerByCuit(cuit, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Clientes creados, con paginado y filtrado por nombre, dni y cuit.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCustomer })]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<string> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.List(filter, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega un Cliente a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateCustomer })]
        public async Task<IActionResult> New([FromBody] DtoEntity model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Add(model, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita un cliente ya creado y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditCustomer })]
        public async Task<IActionResult> Edit([FromBody] DtoEntity model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }
    }
}
