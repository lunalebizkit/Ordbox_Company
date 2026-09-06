using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class DtoSendUserEmail
    {    
        public long Id { get; set; }
        public string Emails { get; set; }
       
    }
}
