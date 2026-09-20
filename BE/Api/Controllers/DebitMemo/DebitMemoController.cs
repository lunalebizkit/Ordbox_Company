using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.ARCA;
using Ordbox.Services.ARCA.Interface;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.DebitMemo
{
    public class DebitMemoController : ApiBaseController
    {
        private readonly DebitMemoService _service;

        private readonly CompanyService _companyService;
        private readonly IArcaIntegracion _arcaIntegracionService;

        public DebitMemoController(DebitMemoService service, CompanyService companyService, IArcaIntegracion arcaIntegracionService)
        {
            _service = service;
            _companyService = companyService;
            _arcaIntegracionService = arcaIntegracionService;
        }

        /// <summary>
        /// Devuelve una ND, buscando en la BASE DE DATOS por ID.
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
        /// Agrega una nueva ND a la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateMemo })]
        public async Task<IActionResult> New([FromBody] DtoRequestDebitMemo model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();

            var debitMemoId = await _service.Add(model, requestedBy).ConfigureAwait(false);

            if (debitMemoId.Success && debitMemoId.Data != null)
            {
                try
                {
                    await GetCAEInvoiceAsync(debitMemoId.Data.Id, requestedBy);
                }
                catch (Exception)
                {
                    return Return(debitMemoId);
                }
            }

            return Return(debitMemoId);
        }

        /// <summary>
        /// Edita una ND y la guarda modificada en la BASE DE DATOS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateMemo })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestDebitMemo model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }

        /// <summary>
        /// Devuelve un listado de ND creadas, con paginado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetMemo })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.List(filter, requestedBy).ConfigureAwait(false));
        }

        #region PRIVATE

        private async Task<IActionResult> GetCAEInvoiceAsync(long id, RequestedBy requestedBy, DateTime? dateTime = null, string? observacion = null)
        {

            var data = await _service.GetById(id, requestedBy).ConfigureAwait(false);

            DtoResponseCompanyCertificate? certificate = await _companyService.GetCompanyCertificateAsync(requestedBy.CompanyId).ConfigureAwait(false);

            if (data.Success && data.Data != null && certificate != null && certificate.IsActive)
            {
                data.Data.DateTime = dateTime == null ? data.Data.DateTime : DateTime.Now;

                if (!string.IsNullOrEmpty(observacion)) { data.Data.Observation = observacion; }

                var responseCAE = await _arcaIntegracionService.CreateDebitNoteAsync(data.Data, certificate).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(responseCAE.Cae) || responseCAE.InvoiceNumber > 0)
                {
                    return Return(await _service.Update(data.Data, responseCAE).ConfigureAwait(false));
                }
            }
            return BadRequest("No se encontro número de factura");
        }

        #endregion
    }
}
