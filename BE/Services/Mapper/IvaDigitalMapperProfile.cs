using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.LibroIvaDigital;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class IvaDigitalMapperProfile : Profile
    {
        public IvaDigitalMapperProfile()
        {
            CreateMap<Invoice, ArchivosTxt>().AfterMap((o, d, c) =>
            {
                d.ArchivoTxtDto = c.Mapper.Map<List<ArchivoTxtDto>>(o.InvoiceDetails)
                ;
            });

            CreateMap<Invoice, ArchivoTxtDto>();
        }

    }
}
