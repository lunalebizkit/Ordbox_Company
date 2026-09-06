using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Ordbox.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;

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
            return Return(await _service.GetById(id));
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
            return Return(await _service.Add(model).ConfigureAwait(false));
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
            return Return(await _service.List(filter).ConfigureAwait(false));
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
            return Return(await _service.ListInactive(filter).ConfigureAwait(false));
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
            return Return(await _service.Update(model).ConfigureAwait(false));
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
            return Return(await _service.GetProductReport().ConfigureAwait(false));
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
            return Return(await _service.Delete(id).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("Activate/{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateProduct })]
        public async Task<IActionResult> Activate(long id)
        {
            return Return(await _service.Active(id).ConfigureAwait(false));
        }
    }
}
