using Ordbox.Services.Common;
using Ordbox.Services.Services;
using Ordbox.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Api.Controllers.Entity
{
    public class EntityController : ApiBaseController
    {
        private readonly EntityService _service;
        public EntityController(EntityService service)
        {
            _service = service;
        }
        /// <summary>
        /// Devuelve una Entidad buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewEntity })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega una nueva Entidad a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateEntity })]
        public async Task<IActionResult> New([FromBody] DtoEntity model)
        {
            return Return(await _service.Add(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita una Entidad ya creada y la guarda modificada en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditEntity })]
        public async Task<IActionResult> Edit([FromBody] DtoEntity model)
        {
            return Return(await _service.Update(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Borra una Entidad buscandolos por el ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.DeleteUser })]
        public async Task<IActionResult> DeleteEntity(long id)
        {
            return Return(await _service.DeleteEntity(id).ConfigureAwait(false));
        }
    }
}
