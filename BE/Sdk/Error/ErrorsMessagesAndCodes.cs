namespace Ordbox.SDK.Error
{
    public class ErrorsCodes
    {
        public const string C_000_MENSAJE_INVALIDO = "000_MENSAJE_INVALIDO";
        public const string C_S001_TOKEN_INVALIDO = "S001_TOKEN_INVALIDO";
        public const string C_S002_CLIENTID_INVALIDO = "S002_CLIENTID_INVALIDO";
        public const string C_002_CLIENTE_INACTIVO = "002_CLIENTE_INACTIVO";
        public const string C_003_TOKEN_INACTIVO = "003_TOKEN_INACTIVO";
        public const string C_004_ELEMENT_NOT_FOUND = "004_ELEMENT_NOT_FOUND";
        public const string C_009_ERROR_DUPLICATE = "009_ERROR_DUPLICATE";
        public const string C_010_ERROR_EXCEPTION = "010_ERROR_EXCEPTION";
        public const string C_999_ERROR_GENERICO = "999_ERROR_GENERICO";
        public const string C_RQ_PRODUCT_REQUEST = "RQ_PRODUCT_REQUEST";
    }
    public class ErrorsMessages
    {
        public static Dictionary<string, string> Dictionary = new Dictionary<string, string>
        {
            { "000_MENSAJE_INVALIDO","El formato del cuerpo de la solicitud es invalido." },
            { "S001_TOKEN_INVALIDO","El token es inválido." },
            { "S002_CLIENTID_INVALIDO","El clientId es inválido." },
            { "002_CLIENTE_INACTIVO","El cliente no esta activo." },
            { "003_TOKEN_INACTIVO","El token fue desactivado." },
            { "004_ELEMENT_NOT_FOUND","No se ha encontrado el elemento" },
            { "005_ERROR_RESULTADO_INVALIDO","El resultado enviado por SAP no es el esperado" },
            { "006_ERROR_TOTE_SAP","El resultado enviado por SAP no es el esperado {0}" },
            { "007_ERROR_SAP_CODE_DUPLICATE","Ya existe un producto con ese código de SAP" },
            { "008_ERROR_CLIENTE_DUPLICATE","Ya existe un cliente con el correo ingresado" },
            { "009_ERROR_DUPLICATE","{0}" },
            { "010_ERROR_EXCEPTION","Ocurrio un error al ejecutar el método" },
            { "011_ERROR_MAIL_NO_VALIDATE","El mail no ha sido validado" },
            { "999_ERROR_MOTOR_ELASTIC","Ups!! Hubo un error intentelo un unos segundos." },
            { "999_ERROR_GENERICO", "Ups!! Hubo un error intentelo un unos segundo." },
            { "RQ_PRODUCT_REQUEST", "---- Producto enviado ----" }
        };

        public static string GetMessage(string key)
        {
            return Dictionary.ContainsKey(key) ? Dictionary[key] : Dictionary[ErrorsCodes.C_999_ERROR_GENERICO];
        }
    }
}
