using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;

namespace Ordbox.Services.Mapper
{
    public class IntegrationLogMapperProfile : Profile
    {
        public IntegrationLogMapperProfile()
        {
            CreateMap<DtoRequestIntegrationLog, IntegrationLog>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => 0));
        }
    }
}
