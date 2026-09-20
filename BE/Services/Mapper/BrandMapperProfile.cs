using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class BrandMapperProfile : Profile
    {
        public BrandMapperProfile()
        {
            CreateMap<Brand, DtoResponseBrand>()
                .AfterMap((o, d, c) =>
                {
                    d.Description = d.Description.ToUpper();
                });
            CreateMap<DtoResponseBrand, Brand>()
                 .AfterMap((o, d, c) =>
                 {
                     d.Description = d.Description.ToUpper();
                 });
        }
    }
}
