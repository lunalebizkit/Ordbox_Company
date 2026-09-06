using Ordbox.SDK.Error;

namespace Ordbox.Services.Common
{
   
    public class OperationResponse<T>
    {
        public OperationResponse(T data, bool success = true, OperationExceptions ex = null)
        {
            Data = data;
            Success = success;
            Exception = ex;
        }

        public bool Success { get; set; }
        public OperationExceptions Exception { get; set; }
        public T Data { get; set; }
    }
}

