using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Quittance
{
    public class QuittanceController : ApiBaseController 
    {
        private readonly QuittanceService _service;
        public QuittanceController(QuittanceService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetQuittance })]
        public async Task<IActionResult> Get(long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetQuittance })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.ListQuittance(filter, requestedBy).ConfigureAwait(false));
        }

        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateQuittance })]
        public async Task<IActionResult> New([FromBody] DtoRequestQuittance model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.NewQuittance(model, requestedBy).ConfigureAwait(false));
        }

        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateQuittance })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestQuittance model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }
    }
}
