using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ordbox.Api.Controllers.Reimprimir
{
    public class ReimprimirController : ApiBaseController
    {
        private readonly ReimprimirDocService _service;

        public ReimprimirController(ReimprimirDocService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateInvoice })]
        public async Task<IActionResult> Reimprimir(ETypeReceipt tipoDocumento, string numeroComprobante)
        {
            return Return(await _service.ReimprmirDoc(tipoDocumento, numeroComprobante).ConfigureAwait(false));
        }
    }
}
