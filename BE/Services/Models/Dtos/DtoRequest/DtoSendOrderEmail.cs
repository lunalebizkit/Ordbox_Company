using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoSendOrderEmail
    {
        public long Id { get; set; }
        public List<string> Emails { get; set; }
    }
}
