using Ordbox.Domain.Enum;

namespace Ordbox.Domain.Model
{
    public partial class Product : BaseModel
    {
        public void UpdateSalePrice(decimal value,bool percent)
        {
            var newPurchasePrice = this.PurchasePrice;

            if (percent)
            {
                newPurchasePrice *= (1 + (value / 100.0m));
            }else
            {
                newPurchasePrice += value;
            }

            this.PurchasePrice = newPurchasePrice;
            this.CashSalePrice = newPurchasePrice * ( 1 + (this.CashSalePercentage / 100.00m));
            this.SalePrice = newPurchasePrice * (1 + (this.SalePercentage / 100.00m));
            this.CardSalePrice = newPurchasePrice * (1 + (this.CardSalePercentage / 100.00m));

        }

        public void UpdatePrecentage(decimal value,int tipo)
        {
            switch (tipo)
            {
                case (int)EPriceProduct.CardSalePercentage:
                    this.CardSalePercentage = value;
                    this.CardSalePrice =this.PurchasePrice * (1 + (this.CardSalePercentage / 100.00m));
                    break;
                case (int)EPriceProduct.CashSalePercentage:
                    this.CashSalePercentage = value;
                    this.CashSalePrice = this.PurchasePrice * (1 + (this.CashSalePercentage / 100.00m));
                    break;
                case (int)EPriceProduct.SalePercentage:
                    this.SalePercentage = (int)value;
                    this.SalePrice = this.PurchasePrice * (1 + (this.SalePercentage / 100.00m));
                    break;

            }
          
        }

        public void UpdateStock(int quantity)
        {
            this.Quantity += quantity;
        }
    }
}
