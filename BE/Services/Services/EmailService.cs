using AutoMapper;
using Ordbox.Domain;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoResponse;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Ordbox.Services.Services
{
    public class EmailService : BaseService
    {

        private IConfiguration _config;
        private IWebHostEnvironment _Env;

        public EmailService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration, IWebHostEnvironment env) :
          base(logger, context, maper, configuration)
        {
            _config = configuration;
            _Env = env;
        }
        ///Email General

        public async Task<OperationResponse<string>> SendEmail(string emailTo, string subject, string htmlBody, string plainBody = "")
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            var response = smtp.Send(email);
            smtp.Disconnect(true);
            return new OperationResponse<string>(response.ToString());
        }

        ///Email de orden

        public async Task<OperationResponse<string>> SendOrder(List<string> emails, string supplierName, string orderNumber, string date, bool paid, List<DtoResponseOrderByIdDetail> details)
        {

            StringBuilder detallesCollection = new(2000);
            foreach (var detail in details)
            {
                detallesCollection.Append($"<tr><td>{detail.ProductName}</td><td style =\"text-align:center\">{detail.ProductCode}</td><td style =\"text-align:center\">{detail.OrderedQuantity}</td><td>${detail.ProductPrice}</td></tr>");
            }

            var emailBody = "<html> " +
                            "<head> " +
                            "<style>" +
                            ".table, th, td {width: 30%; align-items:center; border: 1px solid black;}" +
                            "</style> " +
                           "<div style =\"font-size:37px\"> REFRIGERACIONES DANTE<img style=\"heigth:50px;width:50px;margin-left:100px\" src= https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTWa5Ib3MGd8kiDLloC7s3FaDQfJfRw1oaqOJwBj261Nz0uOOZf1jJ3VZRePSC3IR6KtMw&usqp=CAU></div>" +
                            "</head>" +
                             "<body>" +
                            "<h1>Orden de Pedido</h1>" +
                            "<h3> Hola, " +
                           $"{supplierName}" +
                            "!</br> " +
                            "</h3>" +
                            "<p>" +
                            "Enviamos a continuación el detalle del pedido" +
                            "</p>" +
                            "<p><strong>N° de Pedido</strong>: " +
                           $"{orderNumber}" +
                            "</p>" +
                            "<p><strong>Pedido</strong>: @@paid@@ " +
                            "</p>" +
                            "<p><strong>Fecha del pedido:</strong> " +
                           $"{date}" +
                            "</p>" +
                            "<table>" +
                            "<tr><th> Producto </th><th> Código </th><th> Cantidad </th><th> Precio </th></tr>" +
                           $"{detallesCollection}" +
                            "</table>" +
                            "<p> Esperamos su respuesta.</p>" +
                            "<p> Saludos cordiales! </p>" +
                            "</body>";


            emailBody = emailBody.Replace("@@paid@@", paid ? "Pago" : "No pago");

            foreach (var item in emails)
            {
                await SendEmail(item, "Envio de Pedido", emailBody);
            }

            return new OperationResponse<string>("Ok");

        }
        public async Task<OperationResponse<string>> SendUser(string email, string userName, string password)
        {
            var emailBody = $"<html> <head><div  style=\"font-size:30px\"> Refrigeraciones Dante <img  style=\"heigth:50px;width:50px;margin-left:100px\" src = https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTWa5Ib3MGd8kiDLloC7s3FaDQfJfRw1oaqOJwBj261Nz0uOOZf1jJ3VZRePSC3IR6KtMw&usqp=CAU></div></head>  <body  style=\"padding:25px\" ><h3> <strong> <h2>Hola,{userName}!</h2></br> Queríamos darte la bienvenida a nuestro sistema acercandote la información necesaria para que puedas acceder.</br> <p style=\"padding-left:25px\" ><strong><img style=\"heigth:20px;width:20px\" src=https://cdn-icons-png.flaticon.com/512/149/149071.png>  Usuario:</strong> {userName} </p> <p style=\"padding-left:25px\" ><strong><img style=\"heigth:20px;width:20px\" src=https://w7.pngwing.com/pngs/138/590/png-transparent-computer-icons-password-icon-svg-security-password-icon.png>  Contraseña:</strong> {password} </p></br><p> Saludos! </p></h3><h4>PD:Ante cualquier duda comunicarse con el administrador</h4></strong> </body>";


            await SendEmail(email, "Envio de Datos Usuario", emailBody);

            return new OperationResponse<string>("Ok");

        }

        public async Task<OperationResponse<string>> SendEmailInvoice(string emailTo, byte[] attachment)
        {
            using var smtp = new SmtpClient();

            try
            {
                var email = new MimeMessage();
                string host = _config.GetSection("EmailHost").Value;
                string emailFrom = _config.GetSection("EmailUsername").Value;
                string pass = _config.GetSection("EmailPassword").Value;

                email.From.Add(new MailboxAddress("REFRIGERACION DANTE", emailFrom));
                string templateEail = Path.Combine(_Env.ContentRootPath, "Assets", "body.cshtml");
                string template = File.ReadAllText(templateEail);
                string remplazar = template.Replace("@Model.Year", DateTimeOffset.Now.Year.ToString());

                email.To.Add(MailboxAddress.Parse(emailTo));
                email.Subject = "Refrigeración Dante";
                var body = new TextPart(TextFormat.Html) { Text = remplazar };

                var attachmentPart = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(new MemoryStream(attachment)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = $"Factura_{DateTime.Now:dd-MM-yyyy}.pdf"
                };

                var multipart = new Multipart("mixed");
                multipart.Add(body);
                multipart.Add(attachmentPart);

                email.Body = multipart;

                smtp.CheckCertificateRevocation = false;
                smtp.Connect(host, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(emailFrom, pass);
                var response = smtp.Send(email);
                smtp.Disconnect(true);
                return new OperationResponse<string>("enviado!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsCodes.C_010_ERROR_EXCEPTION, ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex: ex);
                return new OperationResponse<string>(ex.Message);
            }
            finally
            {
                smtp.Disconnect(true);
            }
        }
    }

}

