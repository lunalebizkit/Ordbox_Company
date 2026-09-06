using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ordbox.Api.Controllers.Budget
{
    public class BudgetController : ApiBaseController
    {
        private readonly BudgetService _service;

        public BudgetController(BudgetService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateBudget })]
        public async Task<IActionResult> New([FromBody] DtoRequestBudget model)
        {
            return Return(await _service.New(model).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetBudget })]
        public async Task<IActionResult> Get(long id)
        {
            return Return(await _service.GetById(id).ConfigureAwait(false));
        }

        [HttpPut]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetBudget })]
        public async Task<IActionResult> Edit([FromBody] DtoRequestBudget model)
        {
            return Return(await _service.Update(model).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetBudget })]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<SpecificFilter> filter)
        {
            return Return(await _service.ListBudget(filter).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateBrand })]
        public async Task<IActionResult> Delete(long id)
        {
            return Return(await _service.Delete(id).ConfigureAwait(false));
        }
     
    }
}
