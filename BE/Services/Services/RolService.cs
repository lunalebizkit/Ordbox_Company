using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class RolService : BaseService
    {
        public RolService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
            base(logger, context, maper, configuration)
        { }

        public async Task<OperationResponse<IdResponse<long>>> Add(RequestAddRol model, CancellationToken ct = default)
        {
            try
            {
                model.Id = 0;
                return await AddOrUpdate(model, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<List<DtoResponseRol>>> ListRoles(CancellationToken ct = default)
        {
            try
            {
                return new OperationResponse<List<DtoResponseRol>>(
                    await _contextSql
                            .Rols
                            .AsNoTracking()
                            .Select(p => new DtoResponseRol()
                            {
                                Id = p.Id,
                                Name = p.Name,
                            }).ToListAsync(cancellationToken: ct));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //Agregar o actualizar Rol
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(RequestAddRol model, CancellationToken ct = default)
        {
            try
            {
                var countRols = await _contextSql
                                    .Rols
                                    .AsNoTracking()
                                    .CountAsync(p => p.Name.ToLower() == model.Name.ToLower() && p.Id != model.Id, ct);
                if (countRols > 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "Ya existe un Rol con ese nombre"));
                }

                var rolmodel = new Rol()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Key = model.Key
                };
                if (rolmodel.Id == 0)
                {
                    await _contextSql.Rols.AddAsync(rolmodel, ct).ConfigureAwait(false);
                }
                else
                {
                    var oldModel = await _contextSql
                                    .Rols
                                    .FirstAsync(p => p.Id == rolmodel.Id)
                                    .ConfigureAwait(false);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;

                    _contextSql.Rols.Update(rolmodel);
                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(rolmodel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> Update(RequestAddRol model, CancellationToken ct = default)
        {
            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "El Rol no tiene ID"));
                }

                return await AddOrUpdate(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //Get Rol
        public async Task<OperationResponse<DtoResponseRol>> GetById(long id)
        {
            try
            {
                var rol = await _contextSql
                                    .Rols
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Id == id)
                                    .ConfigureAwait(false);
                if (rol == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoResponseRol>(new OperationExceptions("000", $"Rol no encontrado ID: {id}"));
                }

                var result = _mapper.Map<DtoResponseRol>(rol);

                return new OperationResponse<DtoResponseRol>(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //get Lista de Roles
        public async Task<OperationResponse<List<DtoResponsePermission>>> ListPermissions(CancellationToken ct = default)
        {
            try
            {
                return new OperationResponse<List<DtoResponsePermission>>(
                    await _contextSql
                        .Permissions
                        .AsNoTracking()
                        .Select(p => new DtoResponsePermission()
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Key = p.Key,
                            EnumPermission = p.EnumPermission,
                        }).ToListAsync(cancellationToken: ct));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<List<DtoResponsePermissionRol>>> ListRolPermissions(CancellationToken ct = default)
        {
            try
            {
                var query = await _contextSql.Rols
                    .Include(p => p.PermissionXRols)
                    .ThenInclude(y => y.Permission)
                    .AsNoTracking().ToListAsync(cancellationToken: ct)
                    ;
                var newModel = _mapper.Map<List<DtoResponsePermissionRol>>(query);
                return new OperationResponse<List<DtoResponsePermissionRol>>(newModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdatePermission(DtoRequestAddPermissionXRol model, CancellationToken ct = default)
        {
            try
            {
                var rolesExistentes = await _contextSql
                                        .Rols
                                        .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (rolesExistentes == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_009_ERROR_DUPLICATE));
                    return Error<IdResponse<long>>(new OperationExceptions("009", "No existe un Rol con ese nombre"));
                }

                var oldPermission = await _contextSql.PermissionXRols.Where(p => p.RoleId == model.Id).ToListAsync(cancellationToken: ct);
                _contextSql.PermissionXRols.RemoveRange(oldPermission);

                List<PermissionXRol> permissions = new List<PermissionXRol>();
                foreach (var newPermission in model.PermissionIds)
                {
                    var isPermissions = await _contextSql.Permissions.FirstOrDefaultAsync(p => p.Id == newPermission);

                    if (isPermissions != null)
                    {
                        permissions.Add(new PermissionXRol()
                        {
                            RoleId = model.Id,
                            PermissionId = newPermission
                        });
                    }
                }

                if (permissions.Any())
                {
                    await _contextSql.PermissionXRols.AddRangeAsync(permissions, ct).ConfigureAwait(false);
                }

                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(model.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
    }
}
