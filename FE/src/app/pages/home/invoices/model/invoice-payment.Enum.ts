export enum ePayment{
    cashSalePrice ='Contado',
    salePrice = 'Cuenta Corriente',
    cardSalePrice ='Tarjeta'
}
export enum ePaymentType{
    CONTADO = 1,
    CUENTACORRIENTE = 2,
    TARJETA = 3
}

export const paymentTypes = [{value: 1, label: 'Contado'}, {value: 2, label: 'Cuenta Corriente'}, {value: 3, label: 'Tarjeta'}];

