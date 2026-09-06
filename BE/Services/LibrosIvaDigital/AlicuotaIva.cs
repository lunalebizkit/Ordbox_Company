

namespace Ordbox.Services.LibrosIvaDigital
{
    public class AlicuotaIva
    {
        public List<AlicuotaIvaDto> AlicuotaIvaDto { get; set; }
    }

    public class AlicuotaIvaDto
    {
        #region Tipo de Comprobante
        //3 caracteres
        //Segun tabla de comprobantes

        public string TipoDecComprobante { get; set; }
        #endregion

        #region Punto de Venta
        //5 caracteres 
        public string PuntoDeVenta { get; set; } = "3";
        #endregion  
        
        #region Numero de Comprobante
        //20 caracteres 
        public string NumeroDeComprobante { get; set; }
        #endregion

        #region Importe Neto Gravado
        //15 caracteres 
        public string ImporteNetoGravado { get; set; }
        #endregion

        #region Alicuota Iva
        //4 caracteres 
        public string AlicuotaIva { get; set; }
        #endregion

        #region Impuesto Liquidado
        //15 caracteres 
        public string ImpuestoLiquidado { get; set; }
        #endregion
    }
}
