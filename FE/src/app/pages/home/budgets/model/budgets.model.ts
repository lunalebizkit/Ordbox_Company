export interface BudgetsModel {
    id: number;
    userId: number;
    budgetNumber: number;
    customerName: string;
    payment: string;
    customerAddress: string;
    observation: string;
    dateTime: Date;
    total: number;
    budgetDetails: BudgetDetails[]
}

//BACK
export interface BudgetDetails {
    id: number;
    budgetId: number;
    productId: number;
    productName: string;
    productCode: string;
    quantity: number;
    price: number;


}
//FRONT
export interface BudgetGrid {
    productCode: string;
    productId: number;
    ownCode: number;
    productName: string;
    quantity: number;
    price: number;
    subTotal: number;

}

export function BudgetGridParser(value: any, price: number, quantity: number = 1) {
    return {
        productId: value.id,
        productCode: value.code,
        ownCode: value.id,
        productName: value.description,
        price: price,
        quantity: quantity,
        subTotal: (price * quantity),

    }
}

export function budgetsGridFromParser(value: any, index: number) {
    return {

        productId: (value.productId == -1) ? -(index + 1) : value.productId,
        productCode: value.productCode,
        ownCode: (value.productId == -1) ? -(index + 1) : value.productId,
        productName: value.productName,
        price: value.price,
        quantity: value.quantity,
        subTotal: value.price * value.quantity
    }
}

export function budgetsDetailFromParser(value: any, index: number) {
    return {
        id: value.id,
        productId: (value.productId == -1) ? -(index + 1) : value.productId,
        productCode: value.productCode,
        productName: value.productName,
        price: value.price,
        quantity: value.quantity
    }
}

export function BudgetDetailParser(value: any, price: number, quantity: number = 1) {
    return {
        id: 0,
        budgetId: 0,
        productId: value.id,
        productCode: value.code,
        productName: value.description,
        price: price,
        quantity: quantity
    }
}