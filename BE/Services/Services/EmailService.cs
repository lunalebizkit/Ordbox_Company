using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Ordbox.Domain;
using Ordbox.Domain.Enum;
using Ordbox.SDK.Error;
using Ordbox.SDK.Security;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
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

        public async Task<OperationResponse<string>> SendEmail(string emailTo, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using var smtp = new SmtpClient();
            smtp.Connect(CustomizationConstant.EmailHost, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            var response = smtp.Send(email);
            smtp.Disconnect(true);
            return new OperationResponse<string>(response.ToString());
        }

        ///Email de orden

        public async Task<OperationResponse<string>> SendOrder(List<string> emails, string supplierName, string orderNumber, string date, bool paid, List<DtoResponseOrderByIdDetail> details, string companyName)
        {
            string templateEmail = Path.Combine(AppContext.BaseDirectory, "Assets", "order.cshtml");

            string template = await File.ReadAllTextAsync(templateEmail);

            StringBuilder detailsCollection = new();

            foreach (var detail in details)
            {
                detailsCollection.Append($@"
            <tr>
                <td style=""text-align:left; padding:8px; border:1px solid #dddddd;"">
                    {detail.ProductName}
                </td>

                <td style=""text-align:center; padding:8px; border:1px solid #dddddd;"">
                    {detail.ProductCode}
                </td>

                <td style=""text-align:center; padding:8px; border:1px solid #dddddd;"">
                    {detail.OrderedQuantity}
                </td>

                <td style=""text-align:right; padding:8px; border:1px solid #dddddd;"">
                    ${detail.ProductPrice}
                </td>
            </tr>");
            }

            template = template.Replace("@Model.CompanyName", string.IsNullOrEmpty(companyName) ? string.Empty : companyName);

            template = template.Replace("@Model.Date", date ?? string.Empty);

            template = template.Replace("@Model.Year", DateTimeOffset.Now.Year.ToString());

            template = template.Replace("@Model.Supplier", supplierName ?? string.Empty);

            template = template.Replace("@Model.OrderNumber", orderNumber ?? string.Empty);

            template = template.Replace("@Model.Details", detailsCollection.ToString());
            
            foreach (var item in emails)
            {
                await SendEmail(item, "Envio de Pedido", template);
            }

            return new OperationResponse<string>("Ok");

        }

        public async Task<OperationResponse<string>> SendUser(string email, string userName, string password)
        {
            string templateEail = Path.Combine(AppContext.BaseDirectory, "Assets", "userpass.cshtml");
            string template = File.ReadAllText(templateEail);
            template = template.Replace("@Model.User", userName);
            template = template.Replace("@Model.Pass", password);
            template = template.Replace("@Model.Year", DateTimeOffset.Now.Year.ToString());

            await SendEmail(email, "Envio de Datos Usuario", template);
            return new OperationResponse<string>("Ok");
        }

        public async Task<OperationResponse<string>> SendEmailInvoice(string emailTo, byte[] attachment, DtoResponseCompany company)
        {
            using var smtp = new SmtpClient();

            byte[] passByted = EncryptDecryptWithSeed.GetPasswordBytes();
            byte[]? pfxPassword = EncryptDecryptWithSeed.AESDecrypt(company.CompanyEmailPass, passByted);
            string decryptedPassword = Encoding.UTF8.GetString(pfxPassword);

            if (string.IsNullOrEmpty(company.CompanyEmail) || string.IsNullOrEmpty(decryptedPassword))
            {
                return new OperationResponse<string>(""); ;
            }

            try
            {
                var email = new MimeMessage();
                string host = CustomizationConstant.EmailHost;
                string emailFrom = company.CompanyEmail;
                string pass = decryptedPassword;

                email.From.Add(new MailboxAddress(company.CompanyName, emailFrom));
                string templateEail = Path.Combine(AppContext.BaseDirectory, "Assets", "body.cshtml");
                string template = File.ReadAllText(templateEail);
                template = template.Replace("@Model.CompanyName", string.IsNullOrEmpty(company.CompanyName) ? string.Empty : company.CompanyName);
                string remplazar = template.Replace("@Model.Year", DateTimeOffset.Now.Year.ToString());

                email.To.Add(MailboxAddress.Parse(emailTo));
                email.Subject = company.CompanyName;
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

