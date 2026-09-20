using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Domain.Model
{
    [Table("budget_detail")]
    public  class BudgetDetail : BaseModel
    {

        [Column("budget_id")]
        public long BudgetId { get; set; }
        [ForeignKey(nameof(BudgetId))]
        public Budget? Budget { get; set; }

        [Column("product_id")]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Column("product_name")]
        public string? ProductName { get; set; }

        [Column("product_code")]
        public string? ProductCode { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
      
    }
}
