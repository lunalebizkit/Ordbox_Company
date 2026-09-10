using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Product
{
    public class ProductController : ApiBaseController
    {
        private readonly ProductService _service;
        public ProductController(ProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un Producto buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewProduct })]
        public async Task<IActionResult> GetById(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega un nuevo producto a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateProduct })]
        public async Task<IActionResult> New([FromBody] DtoRequestAddProduct model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Add(model, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Productos creados, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewProduct })]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<ProductFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.List(filter, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un listado de Productos creados, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.DeleteProduct })]
        [Route("ListInactive")]
        public async Task<IActionResult> ListInactive([FromBody] RequestPaginatedData<ProductFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.ListInactive(filter, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita un Producto y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditProduct })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestAddProduct model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un listado de Facturas creadas, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewProduct })]
        public async Task<IActionResult> ProductReport()
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetProductReport(requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Borra un producto buscandolos por el ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.DeleteProduct })]
        public async Task<IActionResult> Delete(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Delete(id, requestedBy).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("Activate/{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateProduct })]
        public async Task<IActionResult> Activate(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Active(id, requestedBy).ConfigureAwait(false));
        }
    }
}
