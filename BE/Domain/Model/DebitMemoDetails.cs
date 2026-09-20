using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordbox.Domain.Model
{
   
        [Table("debit_memo_details")]
        public class DebitMemoDetails : BaseModel
        {

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

            [Column("iva")]
            public decimal Iva { get; set; }

            [Column("debit_memo_id")]
            public long DebitMemoId { get; set; }

            [ForeignKey(nameof(DebitMemoId))]
            public DebitMemo DebitMemo { get; set; }
    }
    }

