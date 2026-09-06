using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class DebitMemoMapperProfile : Profile
    {
        public DebitMemoMapperProfile()
        {
            CreateMap<DtoRequestDebitMemo, DebitMemo>()
                 .AfterMap((o, d, c) =>
                 {
                     d.Total = o.DebitMemoDetails.Sum(p => (p.Quantity * p.Price));
                     d.IvaTotal = o.DebitMemoDetails.Sum(e => e.Quantity * (e.Price - (e.Price / (1 + e.Iva / 100.00m))));
                     d.DateTime = o.DateTime = DateTime.Now;
                 });
            CreateMap<DebitMemo, DtoRequestDebitMemo>();
            CreateMap<DebitMemoDetails, DtoResponseDebitMemoDetails>().ReverseMap();
        }
    }
}
