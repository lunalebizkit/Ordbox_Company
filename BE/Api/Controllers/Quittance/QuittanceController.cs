using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

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
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetQuittance })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            return Return(await _service.ListQuittance(filter).ConfigureAwait(false));
        }

        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateQuittance })]
        public async Task<IActionResult> New([FromBody] DtoRequestQuittance model)
        {
            return Return(await _service.NewQuittance(model).ConfigureAwait(false));
        }

        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateQuittance })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestQuittance model)
        {
            return Return(await _service.Update(model).ConfigureAwait(false));
        }
    }
}
