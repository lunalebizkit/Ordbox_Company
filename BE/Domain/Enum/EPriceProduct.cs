

namespace Ordbox.Domain.Enum
{
 public enum EPriceProduct
    {
        PurchasePrice = 1, //Se suma solo al costo y se calcula todo
        Percentage = 2, // Se suma el porcentaje al costo y se calcula todo
        CashSalePercentage = 3, //Solo se actualiza el porcentaje al precio contado
        CardSalePercentage = 4, //solo se actualiza el porcentake de tarjeta
        SalePercentage = 5, // se actualiza el porcentaje el precio de venta (lista)
    }
}
