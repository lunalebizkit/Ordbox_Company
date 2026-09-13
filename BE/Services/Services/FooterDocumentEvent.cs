using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Ordbox.Services.Services
{
    public class FooterDocumentEvent : PdfPageEventHelper
    {
        private readonly string _footerText;

        public FooterDocumentEvent(string footerText)
        {
            _footerText = footerText;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            cb.BeginText();
            cb.SetFontAndSize(bf, 9);

            // Posición: centrado en el pie de página
            float x = (document.PageSize.Width / 2);
            float y = document.BottomMargin / 2;

            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, _footerText, x, y, 0);
            cb.EndText();

        }
    }
}
