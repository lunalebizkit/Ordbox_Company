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

namespace Ordbox.Services.Services
{
    public class CompanyService : BaseService

    {
        private readonly EmailService _emailService;
        public CompanyService(ErrorManager logger, DBContext context, IMapper maper, IConfiguration configuration, EmailService emailService) :
            base(logger, context, maper, configuration)

        {
            this._emailService = emailService;
        }

        //Agregar Compañia nuevo
        public async Task<OperationResponse<IdResponse<long>>> Add(DtoRequestCompany model, CancellationToken ct = default)
        {
            model.Id = 0;
            return await AddOrUpdate(model, ct).ConfigureAwait(false);
        }
        //Get Compañia
        public async Task<OperationResponse<DtoResponseCompany>> GetById(long id)
        {
            try
            {
                var user = await _contextSql
                                    .Companies
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
                                    .ConfigureAwait(false);

                if (user == null)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_S002_CLIENTID_INVALIDO));
                    return Error<DtoResponseCompany>(new OperationExceptions("001", "El Compañia no es valido"));
                }

                var result = _mapper.Map<DtoResponseCompany>(user);
                return new OperationResponse<DtoResponseCompany>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        //get Lista Compañia
        public async Task<OperationResponse<DtoPagination<DtoResponseCompanyList>>> List(RequestPaginatedData<string> request)
        {
            try
            {
                var query = _contextSql
                                    .Companies
                                    .AsNoTracking()
                                    .Where(p => !p.IsDeleted && (string.IsNullOrEmpty(request.Filter) || p.CompanyName.ToLower().Contains(request.Filter)));

                var count = await query.CountAsync().ConfigureAwait(false);

                var list = await query.OrderBy(p => p.Id)
                                      .Skip(request.Page * request.PageSize)
                                      //.Take(request.PageSize)                                
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                var dto = _mapper.Map<List<DtoResponseCompanyList>>(list);

                return new OperationResponse<DtoPagination<DtoResponseCompanyList>>(new DtoPagination<DtoResponseCompanyList>
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

        //Agregar o actualizar Compañia
        public async Task<OperationResponse<IdResponse<long>>> AddOrUpdate(DtoRequestCompany model, CancellationToken ct = default)
        {
            try
            {
                var newModel = _mapper.Map<Company>(model);

                if (newModel.Id == 0)
                {
                    newModel.CompanyEmailPass = SecurePasswordHasher.Hash(newModel.CompanyEmailPass, 100);
                    await _contextSql.Companies.AddAsync(newModel, ct).ConfigureAwait(false);

                }
                else
                {
                    var oldModel = await _contextSql
                                .Companies
                                .FirstAsync(p => p.Id == newModel.Id, ct)
                                .ConfigureAwait(false);

                    if (!String.IsNullOrEmpty(newModel.CompanyEmailPass))
                    {
                        newModel.CompanyEmailPass = SecurePasswordHasher.Hash(newModel.CompanyEmailPass, 100);
                    }

                    else
                    {
                        newModel.CompanyEmailPass = oldModel.CompanyEmailPass;
                    }
                     _contextSql.Entry(oldModel).CurrentValues.SetValues(newModel);

                }
                await _contextSql.SaveChangesAsync(ct).ConfigureAwait(false);

                return Ok(new IdResponse<long>(newModel.Id));


            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_010_ERROR_EXCEPTION), ex);
                throw;
            }

        }
        //Actualizar Compañia
        public async Task<OperationResponse<IdResponse<long>>> Update(DtoRequestCompany model, CancellationToken ct = default)
        {
            try
            {
                if (model.Id <= 0)
                {
                    _logger.LogWarning(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO));
                    return Error<IdResponse<long>>(new OperationExceptions("000", "El Compañia no tiene ID"));
                }

                return await AddOrUpdate(model, ct).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                throw;
            }
        }
        
        //Elimianr Compañia
        public async Task<OperationResponse<IdResponse<long>>> Delete(long id, CancellationToken ct = default)
        {
            try
            {
                var company = await _contextSql
                                                .Companies
                                                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct)
                                                .ConfigureAwait(false);
                if (company != null)
                {
                    company.IsDeleted = true;

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
