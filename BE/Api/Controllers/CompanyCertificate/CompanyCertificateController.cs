using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.CompanyCertificate
{
    public class CompanyCertificateController : ApiBaseController
    {
        private readonly CompanyService _service;
        public CompanyCertificateController(CompanyService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAccess(Permission = new EPermission[] { EPermission.CreateCompanyCertificate })]
        public async Task<IActionResult> NewCertificate([FromForm] DtoRequestCompanyCertificate model)
        {
            RequestedBy requestedBy = User.GetRequestedBy();

            if (requestedBy.UserRolId != ERol.Admin)
            {
                return Unauthorized("El usuario no tiene permisos para crear un certificado de compañia");
            }

            using var ms = new MemoryStream();
            await model.CertificateData.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            return Return(await _service.AddCertificate(model, fileBytes, requestedBy).ConfigureAwait(false));
        }
        
    }
}
