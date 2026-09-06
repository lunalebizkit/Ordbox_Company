using Ordbox.Services.Models.Dtos.DtoRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Services.Models.Dtos.DtoResponse
{
    public class DtoResponseBudget
    {
        public long Id { get; set; }

        public string UserId { get; set; }

        public long BudgetNumber { get; set; }
        public string? CustomerName { get; set; }

        public string? Payment { get; set; }

        public string? CustomerAddress { get; set; }

        public string? Observation { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Total { get; set; }

        public List<DtoResponseBudgetDetail> BudgetDetails { get; set; }
        
    }

    public class DtoResponseBudgetDetail
    {
        public long Id { get; set; }
        public long BudgetId { get; set; }
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public int Quantity{ get; set; }
        public decimal Price { get; set; }
       

    }


}
