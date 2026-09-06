using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.LibrosIvaDigital;

namespace Ordbox.Services.Mapper
{
    public  class AlicuotaIvaMapperProfile :Profile
    {
        public AlicuotaIvaMapperProfile()
        {
            CreateMap<Invoice, AlicuotaIva>().AfterMap((o, d, c) =>
            {
                d.AlicuotaIvaDto = c.Mapper.Map<List<AlicuotaIvaDto>>(o.InvoiceDetails)
                ;
            });

            CreateMap<Invoice, AlicuotaIvaDto>();
        }
    }
}
