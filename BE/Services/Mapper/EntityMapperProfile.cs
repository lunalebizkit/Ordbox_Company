using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class EntityMapperProfile : Profile
    {
         public EntityMapperProfile()
        {
            CreateMap<Customer, DtoEntity>()
            .ForMember(x => x.EmailEntity, o => o.MapFrom(x => x.EmailEntities))
            
          
            .AfterMap((o, d, c) =>
            {
                d.Name = o.ChangeName(o.Name);
            })
            .ForMember(x => x.PhoneEntity, o => o.MapFrom(x => x.PhoneEntities));

           

            CreateMap<DtoEntity, Customer>()
                .AfterMap((o, d, c) =>
                {
                    d.Dni = o.Dni == 0 ? null : o.Dni;
                    d.Name = d.ChangeName(o.Name);
                });
            CreateMap<DtoEntity, Supplier>()
                .AfterMap((o, d, c) =>
                {
                    d.Dni = o.Dni == 0 ? null : o.Dni;
                }).ReverseMap();
            CreateMap<Supplier,DtoEntityList>()
                 .ForMember(x => x.EmailEntity, o => o.MapFrom(x => x.EmailsStrings))
                 .ForMember(x => x.PhoneEntity, o => o.MapFrom(x => x.PhoneString));
            CreateMap<Customer, DtoEntityList>()
           .ForMember(x => x.EmailEntity, o => o.MapFrom(x => x.EmailsStrings))
           .ForMember(x => x.PhoneEntity, o => o.MapFrom(x => x.PhoneString));

        }
    }
}
