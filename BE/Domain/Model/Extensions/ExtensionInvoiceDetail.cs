

namespace Ordbox.Domain.Model
{
    public partial class Invoice : BaseModel
    {
        public void IvaPrice(decimal value, decimal iva, int quantity)
        {
            decimal newIvaTotal = 0;
            
            if (iva != 0)
            {
                newIvaTotal += (value * (1 + iva / 100.0m)) * quantity;
            }
            this.IvaTotal += newIvaTotal;
        }
    }
}
