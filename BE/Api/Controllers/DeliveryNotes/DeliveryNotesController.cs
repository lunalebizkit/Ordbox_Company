using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.DeliveryNotes
{
    public class DeliveryNotesController : ApiBaseController
    {
        private readonly DeliveryNotesService _service;

        public DeliveryNotesController(DeliveryNotesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.GetById(id, requestedBy).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetRemito })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.ListDeliveryNotes(filter, requestedBy).ConfigureAwait(false));
        }

        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateRemito })]
        public async Task<IActionResult> New([FromBody] DtoRequestDeliveryNotes model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.NewDeliveryNotes(model, requestedBy).ConfigureAwait(false));
        }

        [HttpPut]

        [AllowAccess(Permission = new EPermission[] { EPermission.CreateMemo })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestDeliveryNotes model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            return Return(await _service.Update(model, requestedBy).ConfigureAwait(false));
        }

    }

}
