using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;


namespace Ordbox.Services.Mapper
{
    public class CreditMemoMapper : Profile
    {
        public CreditMemoMapper()
        {
            CreateMap<DtoRequestCreditMemo, CreditMemo>()
                .AfterMap((o, d, c) =>
                {
                    d.Total = o.CreditMemoDetail.Sum(p => (p.Quantity * p.Price));
                    d.IvaTotal = o.CreditMemoDetail.Sum(e => e.Quantity * (e.Price - (e.Price / (1 + e.Iva / 100.00m))) );
                    d.DateTime = o.DateTime = DateTime.Now;
                });

            CreateMap<CreditMemo, DtoRequestCreditMemo>();

            CreateMap<CreditMemoDetail, DtoResponseCreditMemoDetails>().ReverseMap();   
        }
    }
}
