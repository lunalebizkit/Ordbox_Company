using Ordbox.Domain.Enum;
using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.ImpresoraFiscal
{
    public enum eTypeDocumentClient
    {
        Cuit = 1,
        Cuil = 2,
        Dni = 3
    }

    public class ResultOpenInvoice
    {
        public string NroInvoice { get; set; }
    }



    public interface IPrinter
    {
        Task<string> OpenInvoice(ETypeReceipt type, string documentClient, eTypeDocumentClient typeDocument = eTypeDocumentClient.Cuil, string address = "");

        Task<string> OpenND(ETypeReceipt type, string documentClient, eTypeDocumentClient typeDocument = eTypeDocumentClient.Cuil, string address = "");

        Task<string> OpenNC(ETypeReceipt type, string documentClient, eTypeDocumentClient typeDocument = eTypeDocumentClient.Cuil, string address = "");

        Task<string> CerrarJornadaFiscal();

        Task<string> PrintItem(string articulo, double cantidad, decimal monto, decimal iva = 21, string codigo = "9999999");

        Task<string> CloseFactura( int copias = 1, string email = "", bool withRetry = false);

        Task<string> CargarDatosCliente(string customerName, string customerCuit, string customerAddress, ETypeReceipt tipoDocumento );

        Task<string> ReimprimirDocumento( int tipoDocumento,string numeroComprobante );

        Task<DtoResponseConsultarVersion> ConsultarVersionImpresora(ConsultarVersion consultarVersion);

        Task<DtoResponseObtenerReporteElectronico> DownloadPrintReport(ObtenerPrimerBloqueReporteElectronico obtenerPrimerBloque);

        Task<DtoResponseObtenerSiguienteBloqueReporteElectronico> DownloadNextBloquePrintReport(DtoObtenerSiguienteBloqueReporteElectronico obtenerSiguienteBloque);

    }
}
