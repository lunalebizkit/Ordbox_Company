import { InvoiceVersion } from "../../../../common/auth/models/invoice-versions.enum";

export interface DebitMemoModel {
  id: number,
  customerId: number,
  invoiceId: number,
  invoiceNumber: number,
  userId: number,
  debitMemoNumber: number,
  customerName: string,
  customerCuit: string,
  customerAddress: string,
  observation: string,
  dateTime: Date,
  total: number,
  ivaTotal: number,
  type: number;
  caeExpirationTime: Date | null,
  integrationSuccess: boolean | null,
  version: InvoiceVersion,
  cae: string | null,
  debitMemoDetails: DebitMemoDetails[]
}
export interface DebitMemoDetails {
  id: number,
  debitId: number,
  productId: number,
  productName: string,
  productCode: string,
  quantity: number,
  price: number,
  iva: number
}
export interface DebitMemoGrid {
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
export function debitMemoGridParser(value: any, iva: number) {
  return {
    stock: value.quantity,
    productId: value.id,
    code: value.code,
    ownCode: value.id,
    productName: value.description,
    price: value.cashSalePrice,
    quantity: 1,
    subTotal: value.cashSalePrice,
    iva: iva
  }
}

export function debitMemoDetailParser(value: any, iva: number) {
  return {
    id: 0,
    debitId: 0,
    productId: value.id,
    productName: value.description,
    productCode: value.code,
    price: value.cashSalePrice,
    quantity: 1,
    iva: iva
  }
}
export function debitMemoGridFromInvoiceParser(value: any, index: number) {
  return {
    stock: value.quantity,
    productId: (value.productId == -1) ? -(index + 1) : value.productId,
    code: value.productCode,
    ownCode: (value.productId == -1) ? -(index + 1) : value.productId,
    productName: value.productName,
    price: value.price,
    quantity: value.quantity,
    subTotal: value.quantity * value.price,
    iva: value.iva
  }
}

export function debitMemoDetailFromInvoiceParser(value: any, index: number) {
  return {
    id: 0,
    debitId: 0,
    productId: (value.productId == -1) ? -(index + 1) : value.productId,
    productName: value.productName,
    productCode: value.productCode,
    price: value.price,
    quantity: value.quantity,
    iva: value.iva
  }
}  