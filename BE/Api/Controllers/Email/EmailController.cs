using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

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
        /// <param name="plainBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendEmail(string emailTo, string subject, string htmlBody, string plainBody = "")
        {
             _service.SendEmail(emailTo, subject, htmlBody, plainBody);
            return Ok();
        }
    }
}
