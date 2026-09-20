using AutoMapper;
using Ordbox.Domain;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class BaseService
    {
        internal readonly ErrorManager _logger;
        internal readonly IMapper _mapper;
        internal readonly DBContext _contextSql;
        private ErrorManager logger;
        private IMapper maper;
        protected readonly string ConnectionString;

        public BaseService(ErrorManager logger, IMapper maper)
        {
            this.logger = logger;
            this.maper = maper;
        }

        public BaseService(ErrorManager logger,
            DBContext context, IMapper mapper, IConfiguration configuration)
        {
            _logger = logger;
            _contextSql = context;
            _mapper = mapper;
            ConnectionString = configuration.GetConnectionString("sqlconnection");
        }
        public static OperationResponse<T> Error<T>(OperationExceptions error)
        {
            return new OperationResponse<T>(default, false, error);
        }

        public static OperationResponse<T> Error<T>(string code, string message = null)
        {
            var ex = new OperationExceptions(code,
                          string.Format(ErrorsMessages.GetMessage(code), message));
            return new OperationResponse<T>(default, false, ex);
        }

        public static OperationResponse<T> Ok<T>(T data)
        {
            return new OperationResponse<T>(data);
        }
    }
}
