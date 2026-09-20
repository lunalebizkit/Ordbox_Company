using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ordbox.Api.Controllers.Rol
{
    public class RolController : ApiBaseController
    {
        private readonly RolService _service;

        public RolController(RolService service)
        {
            _service = service;
        }

        /// <summary>
        /// Agrega un nuevo Rol a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.RolControl })]

        public async Task<IActionResult> New([FromBody] RequestAddRol model)
        {
            return Return(await _service.Add(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un Rol buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.RolControl })]

        public async Task<IActionResult> Get([FromQuery] long id)
        {
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        /// <summary>
        ///  Edita un Rol y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        //[AllowAccess(Rols = new string[] { ERol.Admin })]
        public async Task<IActionResult> Edit([FromBody] RequestAddRol model)
        {
            return Return(await _service.Update(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Permisos creados, con paginado. 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.RolControl })]
        public async Task<IActionResult> ListPermissions()
        {
            return Return(await _service.ListPermissions().ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega Permisos a un usuario.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.RolControl })]
        public async Task<IActionResult> AddOrUpdatePermission([FromBody] DtoRequestAddPermissionXRol model)
        {
            return Return(await _service.AddOrUpdatePermission(model).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un listado de los Roles y los permisos que contiene cada Rol, con paginado.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.RolControl })]
        public async Task<IActionResult> ListRolPermissions()
        {
            return Return(await _service.ListRolPermissions().ConfigureAwait(false));
        }
    }
}
