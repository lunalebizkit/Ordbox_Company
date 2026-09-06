using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Ordbox.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Api.Controllers.Product
{
    public class PeriodController : ApiBaseController
    {
        private readonly PeriodService _service;
        public PeriodController(PeriodService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un Periodo buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetPeriod })]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve solo los periodos activos buscando en la BASE DE DATOS por fecha.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetPeriod })]
        public async Task<IActionResult> ActivePeriod([FromQuery] DateTime date)
        {
            return Return(await _service.ActivePeriod(date).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un solo periodo buscandolo por fecha en la base de datos.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetPeriod })]
        public async Task<IActionResult> SelectedPeriod([FromBody] RequestPaginatedData<PeriodFilter> filter)
        {
            return Return(await _service.SelectedPeriod(filter).ConfigureAwait(false));
        }

        /// <summary>
        /// Agrega un nuevo Periodo a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreatePeriod })]
        public async Task<IActionResult> New([FromBody] DtoRequestPeriod model)
        {
            return Return(await _service.Add(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de Periodos creados en la BASE DE DATOS, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetPeriod })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<string> filter)
        {
            return Return(await _service.ListPeriods(filter).ConfigureAwait(false));
        }

        /// <summary>
        /// Edita un periodo ya creado y lo guarda modificado en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditPeriod })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestPeriod model)
        {
            return Return(await _service.Update(model).ConfigureAwait(false));
        }

        /// <summary>
        /// Cambia el estado de un periodo a inactivo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [AllowAccess(Permission = new EPermission[] { EPermission.DeletePeriod })]
        public async Task<IActionResult> Delete(long id)
        {
            return Return(await _service.Delete(id).ConfigureAwait(false));
        }
       
    }  
}
