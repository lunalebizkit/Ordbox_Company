using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Ordbox.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ordbox.Api.Controllers.ReporteZ
{
    public class ReporteZController : ApiBaseController
    {
        private readonly ReporteZService _service;

        public ReporteZController(ReporteZService service)
        {
            _service = service;
        }

        /// <summary>
        /// Genera un Cierre de Jornada Fiscal(Reporte Z) en la Impresora Fiscal.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetInvoice })]
        public async Task<IActionResult> CerrarJornadaFiscal()
        {
            return Return(await _service.ReporteZ().ConfigureAwait(false));
        }
        /// <summary>
        /// Retorna la informacion de la impresora fiscal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetInvoice })]
        [Route("printsettings")]
        public async Task<IActionResult> PrintSettings()
        {
            return Return(await _service.PrintSettings().ConfigureAwait(false));
        }
        /// <summary>
        /// Descarga reporte general de la impresora fiscal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAccess(Permission = new EPermission[] { EPermission.GetInvoice })]
        [Route("downloadprintreport")]
        public async Task<IActionResult> DownloadPrintReport([FromQuery] string fechaInicial, [FromQuery] string fechaFinal)
        {
            var bloqueReporteElectronicoBody = new ObtenerPrimerBloqueReporteElectronicoBody()
            {
                FechaFinal = fechaFinal,
                FechaInicial = fechaInicial
            };

            var reporte = await _service.DownloadPrintReport(bloqueReporteElectronicoBody).ConfigureAwait(false);

            if (reporte.Success) {            
                string nombreArchivo = $"ReporteFiscal_{bloqueReporteElectronicoBody.FechaInicial}_{bloqueReporteElectronicoBody.FechaFinal}.zip";
                return File(reporte.Data, "application/zip", nombreArchivo);
            }
            else
            {
                return BadRequest(reporte.Exception?.Info);
            }

        }
    }
}
