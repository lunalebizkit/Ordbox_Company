using AutoMapper;
using Ordbox.Domain;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ordbox.Services.Services
{
    public class EntityService : BaseService
    {
        public EntityService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration) :
          base(logger, context, maper, configuration)

        { }
        public async Task<OperationResponse<DtoEntity>> GetSupplierById(long id)
        {
            try
            {
                var proveedor = await _contextSql
                                 .Suppliers
                                 .Include(x => x.EmailEntities)
                                 .Include(x => x.PhoneEntities)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(p => p.Id == id && !p.IsInactive)
                                 .ConfigureAwait(false);
                if (proveedor == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoEntity>(new OperationExceptions("000", $"Proveedor no encontrado ID: {id}"));
                }

                var result = new DtoEntity()
                {
                    Id = id,
                    Dni = proveedor.Dni,
                    Cuit = proveedor.Cuit,

                    Name = proveedor.Name,
                    Address = proveedor.Address,
                    Observation = proveedor.Observation,
                    EmailEntity = proveedor.EmailEntities.Select(p => p.Email).ToList(),
                    PhoneEntity = proveedor.PhoneEntities.Select(p => p.PhoneNumber).ToList()
                };

                return new OperationResponse<DtoEntity>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }

        public async Task<OperationResponse<DtoEntity>> GetSupplierByCuit(string cuit)
        {
            try
            {
                var entidad = await _contextSql
                                    .Suppliers
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Cuit == cuit && !p.IsInactive)
                                    .ConfigureAwait(false);
                if (entidad == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoEntity>(new OperationExceptions("000", $"Proveedor no encontrado CUIT: {cuit}"));
                }

                var result = _mapper.Map<DtoEntity>(entidad);

                return new OperationResponse<DtoEntity>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<DtoPagination<DtoEntityList>>> ListSupplier(RequestPaginatedData<string> request)
        {
            try
            {
                var query = _contextSql
                                    .Entities.Where(y => !y.IsInactive).OfType<Supplier>()
                                    .AsNoTracking()
                                    .Include(p => p.EmailEntities)
                                    .Include(p => p.PhoneEntities)
                                    .Where(p => p.Name.ToLower().Contains(request.Filter ?? "") ||
                                        p.Dni.ToString().Contains(request.Filter ?? "") ||
                                       p.Cuit.ToLower().Contains(request.Filter ?? "") && !p.IsInactive);

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Name)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                list =list.Where(p => p.Id > 0).ToList();

                var result = _mapper.Map<List<DtoEntityList>>(list);


                return new OperationResponse<DtoPagination<DtoEntityList>>(new DtoPagination<DtoEntityList>
                {
                    Data = result,
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
        public async Task<OperationResponse<IdResponse<long>>> AddSupplier(DtoEntity model, CancellationToken ct = default)
        {
            try
            {
                model.Id = 0;
                if (String.IsNullOrEmpty(model.Address) || model.Dni == 0 || String.IsNullOrEmpty(model.Name))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "Datos incompletos"));
                }
                return await AddOrUpdateSupplier(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> UpdateSupplier(DtoEntity model, CancellationToken ct = default)
        {
            try
            {
                if (model.Id == 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "El proveedor no tiene ID"));
                }
                if (String.IsNullOrEmpty(model.Address) || model.Dni == 0 || String.IsNullOrEmpty(model.Name))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_S001_TOKEN_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("001", "Datos incompletos"));
                }

                return await AddOrUpdateSupplier(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdateSupplier(DtoEntity model, CancellationToken ct = default)
        {
            try
            {
                var entityModel = _mapper.Map<Supplier>(model);

                var email = new EmailEntity();
                var phones = new PhoneEntity();
                if (entityModel.Id == 0)
                {
                    if (model.EmailEntity != null)
                    {

                        foreach (var newEmail in model.EmailEntity)
                        {

                            var emails = new EmailEntity()
                            {
                                Email = newEmail,
                                Entity = entityModel
                            };

                            entityModel.EmailEntities.Add(emails);

                        }
                    }
                    if (model.PhoneEntity != null)
                    {
                        foreach (var newPhone in model.PhoneEntity)
                        {


                            phones = new PhoneEntity()
                            {
                                Entity = entityModel,
                                PhoneNumber = newPhone,
                            };
                            entityModel.PhoneEntities.Add(phones);

                        }
                    }
                    await _contextSql.Suppliers.AddAsync(entityModel, ct).ConfigureAwait(false);
                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                }
                else
                {
                    var oldModel = await _contextSql
                                    .Suppliers
                                    .Include(p => p.EmailEntities)
                                    .Include(p => p.PhoneEntities)
                                    .FirstAsync(p => p.Id == entityModel.Id)
                                    .ConfigureAwait(false);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;
                    _contextSql.Suppliers.Update(entityModel);


                    var oldPhone = await _contextSql.PhoneEntities.AsNoTracking().Where(p => p.EntityId == model.Id).ToListAsync(cancellationToken: ct);

                    _contextSql.PhoneEntities.RemoveRange(oldPhone);

                    var oldMail = await _contextSql.EmailEntities.AsNoTracking().Where(p => p.EntityId == model.Id).ToListAsync(cancellationToken: ct);

                    _contextSql.EmailEntities.RemoveRange(oldMail);

                    if (model.EmailEntity != null)
                    {

                        foreach (var newEmail in model.EmailEntity)
                        {
                            var isMail = await _contextSql.EmailEntities.FirstOrDefaultAsync(p => p.Email == newEmail && p.Id == model.Id);

                            if (isMail == null)
                            {
                                email = new EmailEntity()
                                {
                                    Entity = oldModel,
                                    Email = newEmail,
                                };
                                entityModel.EmailEntities.Add(email);
                            }
                        }
                    }
                    if (model.PhoneEntity != null)
                    {
                        foreach (var newPhone in model.PhoneEntity)
                        {
                            var isPhone = await _contextSql.PhoneEntities.FirstOrDefaultAsync(p => p.PhoneNumber == newPhone && p.Id == model.Id);

                            if (isPhone == null)
                            {
                                phones = new PhoneEntity()
                                {
                                    Entity = oldModel,
                                    PhoneNumber = newPhone,
                                };
                                entityModel.PhoneEntities.Add(phones);
                            }
                        }
                    }

                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                }

                return Ok(new IdResponse<long>(entityModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }

        public async Task<OperationResponse<DtoEntity>> GetById(long id)
        {
            try
            {
                var entidad = await _contextSql
                                    .Customers
                                    .Include(x => x.EmailEntities)
                                    .Include(x => x.PhoneEntities)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsInactive)
                                    .ConfigureAwait(false);
                if (entidad == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoEntity>(new OperationExceptions("000", $"Cliente no encontrado ID: {id}"));
                }

                var result = new DtoEntity()
                {
                    Id = id,
                    Dni = entidad.Dni,
                    Cuit = entidad.Cuit,
                    Name = entidad.Name,
                    Address = entidad.Address,
                    Observation = entidad.Observation,
                    EmailEntity = entidad.EmailEntities.Select(p => p.Email
                    ).ToList(),
                    PhoneEntity = entidad.PhoneEntities.Select(p =>

                       p.PhoneNumber
                    ).ToList(),
                };

                return new OperationResponse<DtoEntity>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        public async Task<OperationResponse<DtoEntity>> GetCustomerByCuit(string cuit)
        {
            try
            {
                var entidad = await _contextSql
                                    .Customers
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Cuit == cuit && !p.IsInactive)
                                    .ConfigureAwait(false);
                if (entidad == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<DtoEntity>(new OperationExceptions("000", $"Cliente no encontrado CUIT: {cuit}"));
                }

                var result = _mapper.Map<DtoEntity>(entidad);

                return new OperationResponse<DtoEntity>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }


        public async Task<OperationResponse<IdResponse<long>>> Add(DtoEntity model, CancellationToken ct = default)
        {
            try
            {
                model.Id = 0;
                if (String.IsNullOrEmpty(model.Address) || model.Dni == 0 || String.IsNullOrEmpty(model.Name))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "Datos incompletos"));
                }
                return await AddOrUpdate(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }

        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoEntity model, CancellationToken ct = default)
        {
            try
            {
                var entityModel = _mapper.Map<Customer>(model);

                var email = new EmailEntity();
                var phones = new PhoneEntity();
                if (entityModel.Id == 0)
                {
                    if (model.EmailEntity != null)
                    {
                        foreach (var newEmail in model.EmailEntity)
                        {
                            var emails = new EmailEntity()
                            {
                                Email = newEmail,
                                Entity = entityModel
                            };
                            entityModel.EmailEntities.Add(emails);

                        }

                    }

                    if (model.PhoneEntity != null)
                    {
                        foreach (var newPhone in model.PhoneEntity)
                        {

                            phones = new PhoneEntity()
                            {
                                Entity = entityModel,
                                PhoneNumber = newPhone
                            };
                            entityModel.PhoneEntities.Add(phones);

                        }
                    }

                    await _contextSql.Customers.AddAsync(entityModel, ct).ConfigureAwait(false);
                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                }
                else
                {
                    var oldModel = await _contextSql
                                    .Customers
                                    .Include(p => p.EmailEntities)
                                    .Include(p => p.PhoneEntities)
                                    .FirstAsync(p => p.Id == entityModel.Id)
                                    .ConfigureAwait(false);

                    _contextSql.Entry(oldModel).State = EntityState.Detached;
                    _contextSql.Customers.Update(entityModel);


                    var oldPhone = await _contextSql.PhoneEntities.Where(p => p.EntityId == model.Id).ToListAsync(cancellationToken: ct);

                    _contextSql.PhoneEntities.RemoveRange(oldPhone);

                    var oldMail = await _contextSql.EmailEntities.Where(p => p.EntityId == model.Id).ToListAsync(cancellationToken: ct);

                    _contextSql.EmailEntities.RemoveRange(oldMail);

                    if (model.EmailEntity != null)
                    {
                        foreach (var newEmail in model.EmailEntity)
                        {
                            var isMail = await _contextSql.EmailEntities.FirstOrDefaultAsync(p => p.Email == newEmail && p.Id == model.Id);

                            if (isMail == null)
                            {
                                email = new EmailEntity()
                                {
                                    Entity = oldModel,
                                    Email = newEmail
                                };
                                entityModel.EmailEntities.Add(email);
                            }
                        }
                    }
                    if (model.PhoneEntity != null)
                    {
                        foreach (var newPhone in model.PhoneEntity)
                        {
                            var isPhone = await _contextSql.PhoneEntities.FirstOrDefaultAsync(p => p.PhoneNumber == newPhone && p.Id == model.Id);

                            if (isPhone == null)
                            {
                                phones = new PhoneEntity()
                                {
                                    Entity = oldModel,
                                    PhoneNumber = newPhone,
                                };
                                entityModel.PhoneEntities.Add(phones);
                            }
                        }
                    }

                    await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);
                }

                return Ok(new IdResponse<long>(entityModel.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }

        }

        public async Task<OperationResponse<DtoPagination<DtoEntityList>>> List(RequestPaginatedData<string> request)
        {
            try
            {
                var query = _contextSql
                                    .Entities.Where(y => !y.IsInactive).OfType<Customer>()
                                    .AsNoTracking()
                                    .Include(p => p.EmailEntities)
                                    .Include(p => p.PhoneEntities)
                                    .Where(p => p.Name.ToLower().Contains(request.Filter ?? "")
                                    || p.Dni.ToString().Contains(request.Filter ?? "")
                                    || p.Cuit.ToLower().Contains(request.Filter ?? ""));



                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Name)
                                      .Skip(request.Page * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

                var result = _mapper.Map<List<DtoEntityList>>(list);


                return new OperationResponse<DtoPagination<DtoEntityList>>(new DtoPagination<DtoEntityList>
                {
                    Data = result,
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
        public async Task<OperationResponse<IdResponse<long>>> Update(DtoEntity model, CancellationToken ct = default)
        {
            try
            {
                if (model.Id == 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "El cliente no tiene ID"));
                }
                if (String.IsNullOrEmpty(model.Address) || model.Dni == 0 || String.IsNullOrEmpty(model.Name))
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_S001_TOKEN_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("001", "Datos incompletos"));
                }
                return await AddOrUpdate(model, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //Elimianr usuario
        public async Task<OperationResponse<IdResponse<long>>> DeleteEntity(long id, CancellationToken ct = default)
        {
            try
            {
                var user = await _contextSql
                                             .Entities
                                             .FirstOrDefaultAsync(p => p.Id == id && !p.IsInactive, ct)
                                              .ConfigureAwait(false);
                if (user.Name.ToLower() == "admin")
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions(ErrorsCodes.C_002_CLIENTE_INACTIVO, "La entidad Admin no se puede eliminar"));

                }
                if (user != null)
                {
                    user.IsInactive = true;

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

    }
}
