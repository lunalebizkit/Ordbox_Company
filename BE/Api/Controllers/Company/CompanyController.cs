using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Company
{
    public class CompanyController : ApiBaseController
    {
        private readonly CompanyService _service;
        public CompanyController(CompanyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Agrega un nuevo Compañia a la BASE DE DATOS. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateCompany })]
        public async Task<IActionResult> New([FromBody] DtoRequestCompany model)
        {
            return Return(await _service.Add(model).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un Compañia buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCompany })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita un Compañia ya creado y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateCompany })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestCompany model)
        {
            return Return(await _service.Update(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Borra un Compañia buscandolos por el ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateCompany })]
        public async Task<IActionResult> Delete(long id)
        {
            return Return(await _service.Delete(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Compañia creados, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewCompany })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<string> filter)
        {
            return Return(await _service.List(filter).ConfigureAwait(false));
        }

    }
}
