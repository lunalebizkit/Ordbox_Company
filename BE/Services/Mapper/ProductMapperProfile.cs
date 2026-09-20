
using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<Product, DtoResponseProduct>()
                 .ForMember(x => x.CategoryName, o => o.MapFrom(y => y.Category.Description))
                 .ForMember(i => i.BrandName, u => u.MapFrom(a => a.Brand.Description))
                 .ForMember(i => i.SupplierName, u => u.MapFrom(a => a.Supplier.Name))
                 .ForMember(i => i.Category, u => u.Ignore())
                 .ForMember(i => i.Brand, u => u.Ignore())
                 .ForMember(i => i.Supplier, u => u.Ignore());
                 
            CreateMap<DtoResponseProduct, Product>();
            CreateMap<DtoRequestAddProduct, Product>().ReverseMap();
            
        }
    }
}
