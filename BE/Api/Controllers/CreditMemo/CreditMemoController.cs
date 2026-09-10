using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.ARCA.Interface;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.CreditMemoController
{
    public class CreditMemoController : ApiBaseController
    {
        private readonly CreditMemoService _service;
        private readonly IArcaIntegracion _arcaIntegracionService;
        private readonly IConfiguration _settingConfiguration;
        public CreditMemoController(CreditMemoService service, IArcaIntegracion arcaIntegracionService, IConfiguration configuration)
        {
            _service = service;
            _arcaIntegracionService = arcaIntegracionService;
            _settingConfiguration = configuration;
        }
        /// <summary>
        /// Devuelve una NC buscando en la BASE DE DATOS por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetMemo })]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Devuelve un listado de NC creadas, con paginado y filtrado por CUIT.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetMemo })]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.List(filter, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Agrega una nueva NC a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateMemo })]
        public async Task<IActionResult> Post([FromBody] DtoRequestCreditMemo model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.NewMemo(model, requestedBy).ConfigureAwait(false));
        }
        /// <summary>
        /// Edita una NC ya creada y la guarda modificada en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateMemo })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestCreditMemo model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }


        /// <summary>
        /// Retorna el log de integración con ARCA de una factura específica, buscando por ID de la factura. Esto incluye detalles de la comunicación, errores y respuestas recibidas durante el proceso de integración.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetInvoice })]
        public async Task<IActionResult> GetIntegrationLogById(long id)
        {
            return Return(await _service.GetIntegrationLogById(id).ConfigureAwait(false));
        }
        #region PRIVATE

        private async Task<IActionResult> GetCAEInvoiceAsync(long id, DateTime? dateTime = null, string? observacion = null)
        {
            if (!bool.Parse(_settingConfiguration.GetSection("ArcaStatus:Status").Value))
            {
                return BadRequest("La impresora esta activada, desactive para realizar el llamado a ARCA");
            }
            RequestedBy requestedBy = User.GetRequestedBy(); //ojo acaaa

            var data = await _service.GetById(id, requestedBy).ConfigureAwait(false);

            if (data.Success && data.Data != null)
            {
                data.Data.DateTime = dateTime == null ? data.Data.DateTime : DateTime.Now;

                if (!string.IsNullOrEmpty(observacion)) { data.Data.Observation = observacion; }

                //var responseCAE = await _arcaIntegracionService.CreateCreditNoteAsync(data.Data).ConfigureAwait(false);

                //if (!string.IsNullOrEmpty(responseCAE.Cae) || responseCAE.InvoiceNumber > 0)
                //{
                //    return Return(await _service.Update(data.Data, responseCAE).ConfigureAwait(false));
                //}
            }
            return BadRequest("No se encontro número de factura");
        }

        #endregion
    }
}
