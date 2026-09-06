import { InvoiceVersion } from "../../../../common/auth/models/invoice-versions.enum";

export interface CreditMemoModel {
  id: number,
  customerId: number,
  userId: number,
  invoiceId: number,
  invoiceNumber: number,
  customerName: string,
  customerCuit: string,
  customerAddress: string,
  creditMemoNumber: number,
  observation: string,
  dateTime: Date,
  total: number,
  ivaTotal: number,
  type: number;
  caeExpirationTime: Date | null,
  integrationSuccess: boolean | null,
  version: InvoiceVersion,
  cae: string | null;
  creditMemoDetail: CreditMemoDetails[]
}
export interface CreditMemoDetails {
  id: number,
  creditId: number,
  productId: number,
  productName: string,
  productCode: number,
  quantity: number,
  price: number,
  iva: number,
}
export interface CreditMemoGrid {
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
export function creditMemoGridParser(value: any, iva: number) {
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

export function creditMemoDetailParser(value: any, iva: number) {
  return {
    id: 0,
    creditId: 0,
    productId: value.id,
    productName: value.description,
    productCode: value.code,
    price: value.cashSalePrice,
    quantity: 1,
    iva: iva
  }
}
export function creditMemoGridFromInvoiceParser(value: any, index: number) {
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

export function creditMemoDetailFromInvoiceParser(value: any, index: number) {
  return {
    id: 0,
    creditId: 0,
    productId: (value.productId == -1) ? -(index + 1) : value.productId,
    productName: value.productName,
    productCode: value.productCode,
    price: value.price,
    quantity: value.quantity,
    iva: value.iva
  }
}
