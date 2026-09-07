using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class CompanyMapperProfile : Profile
    {
        public CompanyMapperProfile() {
        
        CreateMap<Company, DtoResponseCompany>();
        CreateMap<DtoRequestCompany, Company>();
        CreateMap<Company, DtoResponseCompanyList>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CompanyName))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.CompanyOwnerName));
        }
    }
}
