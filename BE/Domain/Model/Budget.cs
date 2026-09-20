using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordbox.Domain.Model
{
    [Table("budget")]
    public class Budget : BaseModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Column("budget_number")]
        public long BudgetNumber { get; set; }

        [Column("customer_name")]
        public string? CustomerName { get; set; }


        [Column("customer_address")]
        public string? CustomerAddress { get; set; }

        [Column("observation")]
        public string? Observation { get; set; }

        [Required]
        [Column("dateTime")]
        public DateTime DateTime { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("payment")]
        public string? Payment { get; set; }
        
        [Column("is_inactive")]
        public bool IsInactive { get; set; }

        [Required]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public ICollection<BudgetDetail> BudgetDetails { get; set; } = new HashSet<BudgetDetail>();
    }
}
