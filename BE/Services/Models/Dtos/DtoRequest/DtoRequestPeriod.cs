

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoRequestPeriod
    {
        public long Id { get; set; }
        public DateTime InitPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public bool Status { get; set; }


    }
}
