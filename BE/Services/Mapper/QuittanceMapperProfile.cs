using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class QuittanceMapperProfile : Profile
    {
        public QuittanceMapperProfile()
        {
            CreateMap<DtoRequestQuittance, Quittance>()
                .AfterMap((o, d, c) =>
                {
                    d.Total = o.QuittanceDetails.Sum(p => p.Total) + d.Cash + o.QuittanceProductDetails.Sum(o => o.Quantity * o.Price);
                });

            CreateMap<DtoRequesQuittanceDetails, QuittanceDetails>().ReverseMap();
            CreateMap<DtoRequestQuittanceProductDetail, QuittanceProductDetails>().ReverseMap();
            //response
            CreateMap<Quittance, DtoResponseQuittance>();

            CreateMap<QuittanceDetails, DtoResponseQuittanceDetails>().ReverseMap();
            CreateMap<QuittanceProductDetails, DtoResponseQuittanceProductDetail>().ReverseMap();
        }
    }
}
