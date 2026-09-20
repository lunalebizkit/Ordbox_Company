using Ordbox.Services.ARCA.Enum;

namespace Ordbox.Services.ARCA.Dto.Request
{
    public class DtoRequestARCACustomer
    {
        public EDocumento TipoDocumento { get; set; }
        public long NumeroDocumento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
    }
}
