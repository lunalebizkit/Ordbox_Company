using iTextSharp.text;
using iTextSharp.text.pdf;
using Ordbox.Domain.Enum;
using Ordbox.Services.ARCA.Enum;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Newtonsoft.Json;
using QRCoder;
using System.Text;

namespace Ordbox.Services.Services
{
    public class FooterWithCAEEvent : PdfPageEventHelper
    {
        private readonly DtoRequestInvoice _invoice;
        private readonly float _marginFromBottom;
        private readonly string _cuit;
        public FooterWithCAEEvent(DtoRequestInvoice invoice, string cuit, float marginFromBottom = 80f)
        {
            _invoice = invoice;
            _marginFromBottom = marginFromBottom;
            _cuit = cuit;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;

            #region Transparencia Fiscal
            Paragraph paragraph = new Paragraph();
            PdfPTable transparenciafiscalTable = new PdfPTable(2)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin
            };
            transparenciafiscalTable.SpacingBefore = 5f;
            float[] columnWidthsTransparenciaFiscal = { 6f, 4f };
            transparenciafiscalTable.SetWidths(columnWidthsTransparenciaFiscal);
            transparenciafiscalTable.AddCell(GenerateTFcell());
            transparenciafiscalTable.AddCell(GenerateTFIvaCell(_invoice));
            paragraph.Add(transparenciafiscalTable);
            #endregion

            #region QR Code
            var table = new PdfPTable(3) { TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin };
            table.SetWidths(new float[] { 1f, 3f, 3f });
            table.AddCell(GenerateQRCell(_invoice, _cuit));
            table.AddCell(GenerateARCALeyend());
            table.AddCell(GenerateCAECell(_invoice));
            #endregion

            float x = document.LeftMargin;
            float y = document.BottomMargin;

            transparenciafiscalTable.WriteSelectedRows(0, -1, x, y + table.TotalHeight - 10f, cb);
            table.WriteSelectedRows(0, -1, x, y + 10f, cb);
        }

        private static PdfPCell GenerateTFIvaCell(DtoRequestInvoice invoice)
        {
            Font fontTextBoldIvas = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.Black);

            Phrase valorresIvaTF = new();
            valorresIvaTF.Add(new Chunk($"IVA contenido {invoice.IvaTotal}", fontTextBoldIvas));
            valorresIvaTF.Add(Chunk.Newline);
            valorresIvaTF.Add(new Chunk(""));
            valorresIvaTF.Add(new Chunk("Otros impuestos nacionales indirectos: 0", fontTextBoldIvas));

            PdfPCell ivaTFcell = new PdfPCell(valorresIvaTF)
            {
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 10f,
                PaddingBottom = 10f,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BackgroundColor = BaseColor.LightGray,
                PaddingRight = 10f
            };

            ivaTFcell.Phrase.Font.Size = 8;

            return ivaTFcell;
        }

        private static PdfPCell GenerateTFcell()
        {
            Font fontTextBoldIvas = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.Black);

            Phrase leyendaTF = new();
            leyendaTF.Add(new Chunk("Régimen de Transparencia Fiscal al Consumidor Ley 27.743", fontTextBoldIvas));
            PdfPCell textCellTF = new PdfPCell(leyendaTF)
            {
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 10f,
                VerticalAlignment = Element.ALIGN_CENTER,
                HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                PaddingBottom = 10f,
                BackgroundColor = BaseColor.LightGray,
                PaddingLeft = 10f
            };
            textCellTF.Phrase.Font.Size = 8;

            return textCellTF;
        }

        private static PdfPCell GenerateARCALeyend()
        {
            var fontARCABold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.Blue);
            var fontARCAleyend = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            var font = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            var phraseArca = new Phrase()
            {
                new Chunk("ARCA", fontARCABold),
                Chunk.Newline,
                new Chunk("Comprobante Autorizado", font),
                Chunk.Newline,
                Chunk.Newline,
                new Chunk("Esta Administracion Federal no se responsabiliza por los datos ingresados en el detalle de la operación", fontARCAleyend)
            };

            var textCellARC = new PdfPCell(phraseArca)
            {
                Border = PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP
            };

            return textCellARC;
        }

        private PdfPCell GenerateQRCell(DtoRequestInvoice invoice, string cuit)
        {
            string qrCode = GenerateQRCode(invoice, cuit);
            byte[] qrCodeImage = GenerateImageQR(qrCode);
            iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(qrCodeImage);
            qrImage.ScaleAbsolute(80f, 80f);

            var qrCell = new PdfPCell(qrImage)
            {
                Border = PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                PaddingBottom = 10f,
                PaddingTop = 1f
            };

            return qrCell;
        }

        private static PdfPCell GenerateCAECell(DtoRequestInvoice invoice)
        {
            var fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var font = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            var phrase = new Phrase();
            phrase.Add(new Chunk("CAE: ", fontBold));
            phrase.Add(new Chunk(invoice.CAE ?? string.Empty, font));
            phrase.Add(new Chunk("  |  Fecha Vto: ", fontBold));
            phrase.Add(new Chunk(invoice.CAEExpirationDate?.ToString("dd/MM/yyyy") ?? string.Empty, font));

            var textCell = new PdfPCell(phrase)
            {
                Border = PdfPCell.TOP_BORDER,
                Padding = 5f,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            return textCell;
        }

        private string GenerateQRCode(DtoRequestInvoice invoice, string cuit)
        {
            var json = new
            {
                ver = 1,
                fecha = invoice.DateTime.ToString("yyyy-MM-dd"),
                cuit = long.Parse(_cuit.Replace("-", "")),
                ptoVta = CustomizationConstant.PuntoDeVenta,
                tipoCmp = MapDocumentType(invoice.Type),
                nroCmp = invoice.InvoiceNumber,
                importe = invoice.Total,
                moneda = CustomizationConstant.TipoMoneda,
                ctz = CustomizationConstant.MonCotiz,
                tipoDocRec = MapPersonIdentificationType(invoice.CustomerCuit),
                nroDocRec = long.Parse(invoice.CustomerCuit),
                tipoCodAut = "E",
                codAut = long.Parse(invoice.CAE)
            };

            string jsonString = JsonConvert.SerializeObject(json);
            string base64Json = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            return $"https://www.afip.gob.ar/fe/qr/?p={base64Json}";
        }

        private static byte[] GenerateImageQR(string data)
        {
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();

            QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCodeImage = new QRCode(qrCodeData);

            var qrCode = new BitmapByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);            
        }

        private static int MapDocumentType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => (int)EInvoiceType.FacturaA,
                ETypeReceipt.EXENTO or ETypeReceipt.B => (int)EInvoiceType.FacturaB,
                _ => invoiceType,
            };
        }

        private static int MapPersonIdentificationType(string customerCuit)
        {
            if (string.IsNullOrEmpty(customerCuit))
            {
                return 0;
            }

            if (customerCuit.Length == 8)
            {
                return (int)EDocumento.DNI;
            }

            if (customerCuit.Length == 11)
            {
                if (customerCuit == CustomizationConstant.DefaultCUIT)
                {
                    return (int)EDocumento.NoCUIT;
                }
                else
                {
                    return (int)EDocumento.CUIT;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}