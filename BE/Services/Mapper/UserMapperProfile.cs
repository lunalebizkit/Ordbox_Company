using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, RequestAddUser>().ReverseMap();
            CreateMap<User, DtoResponseUser>()
                .ForMember(i => i.RoleId, o => o.MapFrom(p => p.Rol.Key));
        }
    }
}
