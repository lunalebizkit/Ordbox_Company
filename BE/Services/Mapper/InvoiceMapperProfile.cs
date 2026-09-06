using AutoMapper;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class InvoiceMapperProfile : Profile
    {
        public InvoiceMapperProfile()
        {
            CreateMap<DtoRequestInvoice, Invoice>()
                .ForMember(destination => destination.Version, option => option.MapFrom(source => CustomizationConstant.CurrentVersion))
                .AfterMap((o, d, c) =>
                {
                    d.IvaTotal = o.InvoiceDetails.Sum(e => (e.Quantity * e.Price) - ((e.Quantity * e.Price) / (1 + (e.Iva / 100.00m)) ));
                });

            CreateMap<Invoice, DtoRequestInvoice>();

            CreateMap<InvoiceDetail, DtoResponseInvoiceDetail>().ReverseMap();

            CreateMap<InvoiceSPReport, DtoResponseInviocesReport>().ReverseMap();

            CreateMap<InvoiceSPReportTotal, DtoResponseInvoiceReportTotals>().ReverseMap();

            CreateMap<Invoice, DtoRequestListInvoice>()
                .ForMember(destination => destination.createdBy, option => option.MapFrom(source => !string.IsNullOrEmpty(source.User.FirstName) ? source.User.FirstName : ""));

        }
    }
}
