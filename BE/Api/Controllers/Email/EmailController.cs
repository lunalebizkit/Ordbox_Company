using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Extension;
using Ordbox.Domain.Model.Extensions;
using Ordbox.Services.Services;

namespace Ordbox.Api.Controllers.Email
{
    public class EmailController : ApiBaseController
    {
        private readonly EmailService _service;
        public EmailController(EmailService service)
        {
            _service = service;
        }

        /// <summary>
        /// Al crear una Orden de compra , envia un email al proveedor avisando el detalle del pedido que se realizo.
        /// </summary>
        /// <param name="emailTo"></param>
        /// <param name="subject"></param>
        /// <param name="htmlBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendEmail(string emailTo, string subject, string htmlBody)
        {
            RequestedBy requestedBy = User.GetRequestedBy();
            _service.SendEmail(emailTo, subject, htmlBody, string.Empty, string.Empty);
            return Ok();
        }
    }
}
