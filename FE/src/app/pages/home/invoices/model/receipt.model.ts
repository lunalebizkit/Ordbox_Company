export interface receiptListModel {
  id: number;
  supplierId: number;
  createdBy: string;
  receiptNumber: number;
  supplierName: string;
  supplierCuit: string;
  supplierAddress: string;
  dateTime: Date;
  total: number;
  type: number;
}

export interface receiptModel {
  id: number;
  supplierId: number;
  userId: number;
  receiptNumber: number;
  supplierName: string;
  supplierCuit: string;
  supplierAddress: string;
  observation: string;
  dateTime: Date;
  total: number;
  ivaTotal: number;
  type: number;
  concNoGravado: number;
  percIva: number;
  percIngBrutos: number;
  receiptDetails: receiptDetails[];
}

export interface receiptDetails {
  id: number;
  receiptId: number;
  productId: number;
  productName: string;
  productCode: string;
  quantity: number;
  price: number;
  iva: number;
}
export interface receiptDetailsGrid {
  ownCode: number,
  productId: number;
  code: string;
  productName: string;
  quantity: number;
  subTotal: number;
  price: number;
  iva: number;
}

export function receiptGridParser(value: any, iva: number) {
  return {
    ownCode: value.id,
    code: value.code,
    productId: value.id,
    productName: value.description,
    price: value.purchasePrice,
    quantity: 1,
    subTotal: value.purchasePrice,
    iva: iva,
  };
}

export function receiptDetailParser(value: any, iva: number) {
  return {
    id: 0,
    receiptId: 0,
    productId: value.id,
    productName: value.description,
    productCode: value.code,
    price: value.purchasePrice,
    quantity: 1,
    iva: iva,
  };
}

export function receiptDetailToGridParser(value: receiptDetails) {
  return {
    ownCode: value.id,
    productId: value.id,
    code: value.productCode,
    productName: value.productName,
    price: value.price,
    quantity: value.quantity,
    subTotal: (value.quantity * value.price),
    iva: value.iva
  }
}
