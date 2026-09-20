using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class DeliveryNotesMapperProfile : Profile
    {
        public DeliveryNotesMapperProfile()
        {
            CreateMap< DtoRequestDeliveryNotes,DeliveryNotes>()
            .AfterMap((o, d, c) =>
             {
                 d.ImportTotal = o.DeliveryNotesDetails.Sum(p => (p.Quantity * p.Price));
             });

            CreateMap<DeliveryNotes, DtoRequestDeliveryNotes>();

            CreateMap<DeliveryNotesDetails, DtoResponseDeliveryNotesDetail>().ReverseMap();
        }
    }
}
