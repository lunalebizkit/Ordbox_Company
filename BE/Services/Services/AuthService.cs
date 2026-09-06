using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.SDK.Security;
using Ordbox.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class AuthService : BaseService
    {
        public AuthService(ErrorManager logger, DBContext context, IMapper mapper, IConfiguration config) :
           base(logger, context, mapper, config)
        { }

        public async Task<OperationResponse<User>> GetUserLogin(string email, string password)
        {
            try
            {
              var user = await _contextSql.Users
                                        .AsNoTracking()
                                        .Include(p => p.Rol)
                                        .FirstOrDefaultAsync(p => p.Email == email && !p.IsDeleted)
                                        .ConfigureAwait(false);

                if (user == null || !SecurePasswordHasher.Verify(password, user.Password))
                {
                    return Error<User>(new OperationExceptions("001", "El usuario no es válido"));
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetUserLogin", ex: ex);
                throw;
            }
        }
    }
}
