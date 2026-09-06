using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;


namespace Ordbox.Services.Mapper
{
    public class ReceiptMapperProfile :Profile
    {
        public ReceiptMapperProfile()
        {
            CreateMap<DtoRequestReceipt, Receipt>()
                .AfterMap((o, d, c) =>
                {
                    d.Total = o.ConcNoGravado + o.PercIngBrutos + o.PercIva + o.ReceiptDetails.Sum(p => (p.Quantity * p.Price) );
                    d.IvaTotal = o.ReceiptDetails.Sum(e => e.Quantity *( e.Price - (e.Price /( 1 + e.Iva / 100.00m))) );
                    d.IsInactive = false;
                });

            CreateMap<Receipt, DtoRequestReceipt>().ReverseMap();

            CreateMap<Receipt, DtoRequestListReceipt>()
                .ForMember(destiantion => destiantion.CreatedBy, option => option.MapFrom(source => !string.IsNullOrEmpty(source.User.FirstName) ? source.User.FirstName : ""));

            CreateMap<ReceiptDetails, DtoResponseReceiptDetail>().ReverseMap();

        }
    }
}
