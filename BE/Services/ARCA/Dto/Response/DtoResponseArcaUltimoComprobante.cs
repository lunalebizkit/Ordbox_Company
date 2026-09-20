namespace Ordbox.Services.ARCA.Dto.Response
{
    public class DtoResponseArcaUltimoComprobante
    {
        public string CbteNro { get; set; }
        public List<string> Observaciones { get; set; } = new();
        public List<string> Errores { get; set; } = new();
    }
}
