using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Api.Model;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.User
{
    public class UserController : ApiBaseController
    {
        private readonly UserService _service;
        public UserController(UserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Agrega un nuevo Usuario a la BASE DE DATOS. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateUser })]
        public async Task<IActionResult> New([FromBody] RequestAddUser model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Add(model, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un Usuario buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewUser })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita un Usuario ya creado y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditUser })]
        public async Task<IActionResult> Edit([FromBody] RequestAddUser model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Borra un Usuario buscandolos por el ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.DeleteUser })]
        public async Task<IActionResult> Delete(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Delete(id, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Usuario creados, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.ViewUser })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<string> filter)
        {
            return Return(await _service.ListUsers(filter).ConfigureAwait(false));
        }

    }
}
