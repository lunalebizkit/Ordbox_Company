using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.LibroIvaDigital
{

    public class ArchivosTxt
    {
        public List<ArchivoTxtDto> ArchivoTxtDto { get; set; }
    }
    public class ArchivoTxtDto
    {
        #region Fecha De Comprobante

        //AAAAMMDD
        public string? FechaDeComprobante { get; set; }

        #endregion

        #region Tipo De Comprobante
        //Segun tabla Comprobante Ventas

        public string? TipoDeComprobante { get; set; }

        #endregion

        #region Punto De Ventas

        public string PuntoDeVenta { get; set; } = "00003";

        #endregion

        #region Numero De Comprobante

        public string? NumeroDeComprobante { get; set; }

        #endregion

        #region Numero De ComprobanteHasta

        public string? NumeroDeComprobanteHasta { get; set; }

        #endregion

        #region Codigo de Documento del Comprador(Cuit)
        //Segun tabla de Documentos

        public string CodigoDocumento { get; set; }

        #endregion

        #region Numero de Identificacion del Comprador
        //Completar con ceros a la izquierda

        public string? NumeroDeIdentificacionComprador { get; set; }

        #endregion

        #region Apellido y Nombre o denominacion del Comprador

        public string? NombreCompletoComprador { get; set; }

        #endregion

        #region Importe Total de la Operacion
        // 13 enteros 2 decimal sin punto decimal

        public string? ImporteTotal { get; set; }

        #endregion

        #region Importe Total de Conceptos que no integran el precio neto gravado
        //13 enteros 2 decimales sin punto decimal

        public string? NetoGravado { get; set; } = "000000000000000";

        #endregion

        #region Percepcion a no categorizados
        //13 enteros 2 decimales sin punto decimal

        public string? NoCategorizados { get; set; } = "000000000000000";

        #endregion

        #region Importe de operaciones exentas
        //13 enteros 2 decimales sin punto decimal

        public string? OperacionesExentas { get; set; } = "000000000000000";

        #endregion

        #region Importe de percepciones o pagos a cuenta de Impuesto Nacionales
        //13 enteros 2 decimales sin punto decimal

        public string? ImpuestosNacionales { get; set; } = "000000000000000"; 

        #endregion

        #region Importe de percepcion de Ingresos Brutos
        //13 enteros 2 decimales sin punto decimal

        public string? IngresosBrutos { get; set; } = "000000000000000";

        #endregion

        #region Importe de percepciones de Impuestos Municipales
        //13 enteros 2 decimales sin punto decimal

        public string? ImpuestosMunicipales { get; set; } = "000000000000000";

        #endregion

        #region Importe de Impuestos Internos
        //13 enteros 2 decimales sin punto decimal

        public string? ImpuestosInternos { get; set; } = "000000000000000";

        #endregion

        #region Codigo de Moneda
        //Segun tabla tipo de Monedad PES   

        public string? CodigoDeMoneda { get; set; } = "PES";

        #endregion

        #region Tipo de Cambio
        //4 enteros 6 decimales sin punto decimal

        public string TipoDeCambio { get; set; } = "0001000000";

        #endregion

        #region Cantidad De Alicuotas de IVA

        public string? AlicuotaIva { get; set; } = "1";

        #endregion

        #region Codigo de Operacion
        //Segun tabla de Codigo de Operacion

        public string CodigoDeOperacion { get; set; } = "0";

        #endregion

        #region OtrosTributos
        //13 enteros 2 decimales sin punto de decimal

        public string? OtrosTributos { get; set; } = "000000000000000";

        #endregion

        #region Fecha de Vencimiento o Pago
        //AAAAMMDD

        public string? FechaDePago { get; set; }

        #endregion
    }
}