export class quittanceModel {
    constructor(
        public id: number,
        public quittanceNumber: number,
        public customerName: string,
        public address: string,
        public customerCuit: string,
        public dateTime: Date,
        public amount: string,
        public concept: string,
        public total: number,
        public cash: number,
        public quittanceDetails: QuittanceDetails[],
        public quittanceProductDetails: QuittanceProductDetails[],
    ) { }

}
export class QuittanceDetails {
    constructor(
        public id: number,
        public quittanceId: number,
        public total: number,
        public quantity: number,
        public bank: string,
        public checkNumber: string
    ) { }

}

export class QuittanceProductDetails {
    constructor(
        public id: number,
        public quittanceId: number,
        public productId: number,
        public productName: string,
        public productCode: number | null,
        public quantity: number,
        public price: number,
        public iva: number,) { }
}
export interface QuittanceGrid {
    stock: number,
    productId: number;
    ownCode: number;
    productCode: number;
    productName: string;
    quantity: number;
    price: number;
    subTotal: number;
    iva: number;

}
// Funcion para parsear datos al grid de pantalla
export function quittanceGridParser(value: any, iva: number) {
    return {
        stock: value.quantity,
        productId: value.id,
        productCode: value.code,
        ownCode: value.id,
        productName: value.description,
        price: value.cashSalePrice,
        quantity: 1,
        subTotal: value.cashSalePrice,
        iva: iva
    }
}

// Funcion para parsear los datos al modelo de DB
export function quittanceDetailParser(value: any, iva: number, quantity: number = 1) {
    return {
        id: 0,
        quittanceId: 0,
        productId: value.id,
        productName: value.description,
        productCode: value.code,
        price: value.cashSalePrice,
        quantity: quantity,
        iva: iva
    }
}


export function quittancesGridFromParser(value: any, index: number) {
    return {
        productId: (value.productId == -1) ? -(index + 1) : value.productId,
        productCode: value.productCode,
        ownCode: (value.productId == -1) ? -(index + 1) : value.productId,
        productName: value.productName,
        price: value.price,
        quantity: value.quantity,
        subTotal: value.price * value.quantity,
        iva: value.iva
    }
}

export function quittancesDetailFromParser(value: any, index: number) {
    return {
        productId: (value.productId == -1) ? -(index + 1) : value.productId,
        productCode: value.productCode,
        ownCode: (value.productId == -1) ? -(index + 1) : value.productId,
        productName: value.productName,
        price: value.price,
        quantity: value.quantity,
        iva: value.iva,
        id: value.id,
    }
}