using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class OrderSupplierMapperProfile : Profile
    {
        public OrderSupplierMapperProfile()
        {
            CreateMap<SupplierOrder, DtoResponseSupplierOrder>()
                .ForMember(o => o.SupplierName, x => x.MapFrom(y => y.Supplier.Name))
                .ForMember(o => o.Observation, x => x.MapFrom(source => (string.IsNullOrEmpty(source.Observation) ? null : source.Observation.Trim())))
            .AfterMap((o, d, c) =>
            {
                d.OrderDetail = c.Mapper.Map<List<DtoResponseOrderDetail>>(o.SupplierOrderDetail);
            });
            CreateMap<DtoRequestSupplierOrder, SupplierOrder>()
                .ForMember(destination => destination.Observation, option => option.MapFrom(source => (string.IsNullOrEmpty(source.Observation) ? null : source.Observation.Trim())))
                .AfterMap((o, d,c)=> {
                    d.DateTime = o.DateTime=  DateTime.Now;
                    d.ScheduledDate = o.DateTime=  DateTime.Now;
                    d.SupplierOrderDetail = c.Mapper.Map<List<SupplierOrderDetail>>(o.OrderDetail);
                 });          
            CreateMap<SupplierOrderDetail, DtoResponseOrderDetail>()
                .ForMember( o => o.ProductId, x => x.MapFrom(y => y.Product.Description));
            CreateMap<DtoRequestOrderDetail, SupplierOrderDetail>();

            CreateMap<SupplierOrder, DtoResponseSupplierOrderById>()
                .ForMember(o => o.SupplierEmail, x => x.MapFrom(y => y.Supplier.EmailEntities.Select(p=> p.Email)))
                .ForMember(o => o.SupplierName, x => x.MapFrom(y => y.Supplier.Name))
                .AfterMap((o, d, c)=>
                {
                    d.OrderDetail = c.Mapper.Map<List<DtoResponseOrderByIdDetail>>(o.SupplierOrderDetail);
                });

            CreateMap<SupplierOrderDetail, DtoResponseOrderByIdDetail>()
                .ForMember(o => o.ProductCode, x => x.MapFrom(y => y.Product.Code))
                .ForMember(o => o.ProductName, x => x.MapFrom(y => y.Product.Description))
                .ForMember(o => o.ProductPrice, x => x.MapFrom(y => y.Product.PurchasePrice));

        }
    }
}
