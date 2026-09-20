

using AutoMapper;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;

namespace Ordbox.Services.Mapper
{
    public class PeriodMapperProfile : Profile
    {
        public PeriodMapperProfile()
        {
            CreateMap<DtoRequestPeriod, Period>()
                .AfterMap((o, d, c) => {
                d.EndPeriod = o.EndPeriod.Date > o.InitPeriod.Date ? o.EndPeriod : o.EndPeriod.AddMonths(1).AddSeconds(-1);
        });
            CreateMap<Period, DtoResponsePeriod>();
        }
    }
}
