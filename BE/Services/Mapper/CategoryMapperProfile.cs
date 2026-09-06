using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class CategoryMapperProfile : Profile
    {
        public CategoryMapperProfile()
        {
            CreateMap<Category, DtoResponseCategory>()
                .AfterMap((o,d,c)=>
                {
                    d.Description = d.Description?.ToUpper();
                });

        }
    }
}
