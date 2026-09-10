using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Category
{
    public class CategoryController : ApiBaseController
    {
        private readonly CategoryService _service;
        public CategoryController(CategoryService service)
        {
            _service = service;
        }
        /// <summary>
        /// Devuelve una Categoria, buscando en la BASE DE DATOS por ID. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCategory })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Agrega una Categoria a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateCategory })]
        public async Task<IActionResult> New([FromBody] DtoResponseCategory model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Add(model, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Edita una Categoria ya creada y la guarda modifica en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditCategory })]
        public async Task<IActionResult> Edit([FromBody] DtoResponseCategory model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un listado de Categorias creadas, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCategory })]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<string> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.ListCategory(filter, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Borra una Categoría buscandolos por el ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.DeleteBrand })]
        public async Task<IActionResult> Delete(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Delete(id, requestedBy).ConfigureAwait(false));
        }
    }
}
