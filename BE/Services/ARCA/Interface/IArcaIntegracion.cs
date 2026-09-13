using Ordbox.Services.ARCA.Dto.Response;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.ARCA.Interface
{
    public interface IArcaIntegracion
    {
        Task<DtoResponseARCAInvoice> CrearComprobanteAsync(DtoRequestInvoice invoice, DtoResponseCompanyCertificate certificate, CancellationToken ct = default);

    }
}
