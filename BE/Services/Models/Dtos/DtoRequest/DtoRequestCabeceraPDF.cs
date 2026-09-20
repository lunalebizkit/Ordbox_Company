namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestCabeceraPDF
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Cuit { get; set; }
        public string ?Observacion { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }

    }
}
