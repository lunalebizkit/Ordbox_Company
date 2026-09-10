using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.SDK.Security;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordbox.Domain.Model.Extensions;

namespace Ordbox.Services.Services
{
    public class UserService : BaseService

    {
        private readonly EmailService _emailService;
        public UserService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration, EmailService emailService) :
            base(logger, context, maper, configuration)

        {
            this._emailService = emailService;
        }
        //Login de Usuario
        public async Task<OperationResponse<User>> GetUserLogin(string userName, string password)
        {
            try
            {
                var user = await _contextSql
                         .Users
                         .AsNoTracking()
                         .Include(x => x.Company)
                         .Include(x => x.Rol)
                         .ThenInclude(y => y.PermissionXRols)
                         .ThenInclude(y => y.Permission)
                         .FirstOrDefaultAsync(x => x.UserName == userName && !x.IsDeleted)
                         .ConfigureAwait(false);


                if (user == null || !SecurePasswordHasher.Verify(password, user.Password))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_S002_CLIENTID_INVALIDO), userName);
                    return Error<User>(new OperationExceptions("001", "El usuario no es valido"));

                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }
        //Agregar usuario nuevo
        public async Task<OperationResponse<IdResponse<long>>> Add(RequestAddUser model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);
        }
        //Get usuario
        public async Task<OperationResponse<DtoResponseUser>> GetById(long id)
        {
            try
            {
                var user = await _contextSql
                                    .Users
                                    .Include(p => p.Rol)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
                                    .ConfigureAwait(false);

                if (user == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_S002_CLIENTID_INVALIDO));
                    return Error<DtoResponseUser>(new OperationExceptions("001", "El usuario no es valido"));
                }

                var result = _mapper.Map<DtoResponseUser>(user);
                return new OperationResponse<DtoResponseUser>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //get Lista Usuario
        public async Task<OperationResponse<DtoPagination<DtoResponseUser>>> ListUsers(RequestPaginatedData<string> request)
        {
            try
            {
                var query = _contextSql
                                    .Users.Include(P => P.Rol)
                                    .AsNoTracking()
                                    .Where(p => ((p.FirstName.ToLower().Contains(request.Filter ?? "")) || (p.LastName.ToLower().Contains(request.Filter ?? "")))
                                                  && !p.IsDeleted);


                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.FirstName)
                                      .Skip(request.Page * request.PageSize)
                                      //.Take(request.PageSize)                                
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                var dto = _mapper.Map<List<DtoResponseUser>>(list);

                return new OperationResponse<DtoPagination<DtoResponseUser>>(new DtoPagination<DtoResponseUser>
                {
                    Data = dto,
                    PageSize = request.PageSize,
                    TotalCount = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        //Agregar o actualizar usuario
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(RequestAddUser model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                var countEmails = await _contextSql
                                    .Users
                                    .AsNoTracking()
                                    .CountAsync(p => p.Email.ToLower() == model.Email.ToLower() && p.Id != model.Id && !p.IsDeleted && p.UserName == model.UserName, ct);
                if (countEmails > 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "Ya existe un usuario con ese correo"));
                }

                var usermodel = _mapper.Map<User>(model);

                var email = await _emailService.SendUser(model.Email, model.UserName, model.Password);

                if (usermodel.Id == 0)
                {
                    usermodel.Password = SecurePasswordHasher.Hash(usermodel.Password, 100);
                    await _contextSql.Users.AddAsync(usermodel, ct).ConfigureAwait(false);

                }
                else
                {
                    var oldUser = await _contextSql
                                .Users
                                .FirstAsync(p => p.Id == usermodel.Id, ct)
                                .ConfigureAwait(false);

                    if (!String.IsNullOrEmpty(usermodel.Password))
                    {
                        usermodel.Password = SecurePasswordHasher.Hash(usermodel.Password, 100);
                    }

                    else
                    {
                        usermodel.Password = oldUser.Password;
                    }

                    usermodel.CompanyId = requestedBy.UserRolId == Domain.Enum.ERol.Admin ? usermodel.CompanyId : oldUser.CompanyId;                    

                     _contextSql.Entry(oldUser).CurrentValues.SetValues(usermodel);

                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(usermodel.Id));


            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex);
                throw;
            }

        }
        //Actualizar usuario
        public async Task<OperationResponse<IdResponse<long>>> Update(RequestAddUser model, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "El usuario no tiene ID"));
                }

                return await AddOrUpdate(model, requestedBy, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //Elimianr usuario
        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, RequestedBy requestedBy, CancellationToken ct = default)
        {
            try
            {
                var user = await _contextSql
                                             .Users
                                             .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct)
                                             .ConfigureAwait(false);
                if (user != null)
                {
                    user.IsDeleted = true;

                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_002_CLIENTE_INACTIVO));
                    return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_002_CLIENTE_INACTIVO, ErrorsMessages.GetMessage(ErrorsCodes.C_002_CLIENTE_INACTIVO)));
                }

                return Ok(new IdResponse<long>(id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }


        public async Task<OperationResponse<IdResponse<long>>> AddAuthRefresh(AuthRefresh authRefresh)
        {
            try
            {
                _contextSql.AuthRefreshes.Add(authRefresh);
                await _contextSql.SaveChangesAsync().ConfigureAwait(false);
                return Ok(new IdResponse<long>(authRefresh.UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<AuthRefresh>> GetAuthRefreshByHash(string tokenHash)
        {
            try
            {
                var authRefresh = await _contextSql
                    .AuthRefreshes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.RevokedAt == null)
                    .ConfigureAwait(false);

                if (authRefresh == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_S002_CLIENTID_INVALIDO));
                    return Error<AuthRefresh>(new OperationExceptions(ErrorsCodes.C_003_TOKEN_INACTIVO, ErrorsMessages.GetMessage(ErrorsCodes.C_003_TOKEN_INACTIVO)));
                }
                return Ok(authRefresh);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<User?>> GetUserById(long id)
        {
            try
            {
                var user = await _contextSql
                         .Users
                         .AsNoTracking()
                         .Include(x => x.Rol)
                         .ThenInclude(y => y.PermissionXRols)
                         .ThenInclude(y => y.Permission)
                         .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted)
                         .ConfigureAwait(false);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }

        public async Task<OperationResponse<bool>> UpdateAuthRefresh(AuthRefresh authRefresh)
        {
            try
            {
                _contextSql.AuthRefreshes.Update(authRefresh);
                await _contextSql.SaveChangesAsync().ConfigureAwait(false);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex: ex);
                throw;
            }
        }
    }
}
