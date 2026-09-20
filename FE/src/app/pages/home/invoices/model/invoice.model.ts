import { InvoiceVersion } from "../../../../common/auth/models/invoice-versions.enum";

export interface InvoiceListModel {
    id: number;
    customerId: number;
    createdBy: string;
    invoiceNumber: number;
    cae: string;
    customerName: string;
    customerCuit: string;
    customerAddress: string;
    dateTime: Date;
    total: number;
    ivaTotal: number;
    type: number;
    ivaSelected: number;
    caeExpirationTime: Date | null,
    integrationSuccess: boolean | null,
    version: InvoiceVersion
}

export interface InvoiceModel {
    id: number;
    customerId: number;
    userId: number;
    invoiceNumber: number;
    cae: string | null;
    customerName: string;
    customerCuit: string;
    customerAddress: string;
    customerEmail: string | null;
    observation: string;
    dateTime: Date;
    iva21: number;
    iva27: number;
    iva10: number;
    total: number;
    ivaTotal: number;
    type: number;
    ivaSelected: number;
    caeExpirationTime: Date | null;
    integrationSuccess: boolean | null;
    version: InvoiceVersion;
    invoiceDetails: InvoiceDetails[]
}
export interface InvoiceDetails {
    id: number;
    invoiceId: number;
    productId: number;
    productName: string;
    productCode: number | null;
    quantity: number;
    price: number;
    iva: number;
}
export interface InvoiceDetailList {
    stock: number,
    productId: number;
    ownCode: number;
    code: number;
    productName: string;
    quantity: number;
    price: number;
    subTotal: number;
    iva: number;
}

export function invoiceGridParser(value: any, iva: number, price: number, quantity: number = 1) {
    return {
        stock: value.quantity,
        productId: value.id,
        code: value.code,
        ownCode: value.id,
        productName: value.description,
        price: price,
        quantity: quantity,
        subTotal: (price * quantity),
        iva: iva
    }
}

export function invoiceDetailParser(value: any, iva: number, price: number, quantity: number = 1) {
    return {
        id: 0,
        invoiceId: 0,
        productId: value.id,
        productName: value.description,
        productCode: value.code,
        price: price,
        quantity: quantity,
        iva: iva
    }
}

export function invoiceDetailToGridParser(value: InvoiceDetails) {
    return {
        stock: value.quantity,
        productId: value.id,
        code: value.productCode,
        ownCode: value.id,
        productName: value.productName,
        price: value.price,
        quantity: value.quantity,
        subTotal: (value.quantity * value.price),
        iva: value.iva
    }
}

export interface InvoiceReportTotal {
    invoiceDate : Date;
    totalQuantity: number;
    totalPrice: number;
    totalSubTotal : number;
    invoicesReports: InvoiceReport[]

}
export interface InvoiceReport {
    date : Date;
    productName: string;
    customerName: string;
    quantity : number;
    price: number; 
    subTotal: number; 
}
