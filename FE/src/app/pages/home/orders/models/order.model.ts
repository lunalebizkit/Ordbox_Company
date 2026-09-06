
export interface NewOrder {
  id: number;
  supplierId: number;
  observation: string;
  supplierName: string | null;
  isPaid: boolean;
  dateTime: Date;
  statusId: number;
  supplierEmail: string[];
  orderDetail: NewOrderDetail[];
}
export interface NewOrderDetail {
  id: number;
  productId: number | string;
  orderedQuantity: number;
  recievedQuantity: number;
  statusId: number;
}

export interface OrderDetailGrid {
  id: number;
  code: string;
  productName: string;
  orderedQuantity: number;
  price: number;
  subTotal: number;
  recievedQuantity: number;
}

export function orderGridParser(value: any) {
  return {
    code: value.productCode,
    id: value.productId,
    productName: value.productName,
    price: value.productPrice,
    orderedQuantity: value.orderedQuantity,
    subTotal: Number(value.productPrice)  * Number(value.orderedQuantity),
    recievedQuantity: value.recievedQuantity,
  };}

export function orderGridProductParser(value: any) {
    return {
      code: value.code,
      id: value.id,
      productName: value.description,
      price: value.purchasePrice,
      quantity: 1,
      subTotal: Number(value.purchasePrice)  * 1,
      recievedQuantity: 1,
      orderedQuantity: 1,
      
    };
};
export function orderNewProductParser(value: any) {
  return {
    productId: value.id,
    id: 0,
    orderedQuantity: 1,
    recievedQuantity: 1,
    statusId: 3,
  };
};
export function orderOldProductParser(value: any) {
  return {
    productId: value.productId,
    id: value.id,
    orderedQuantity: value.orderedQuantity,
    recievedQuantity: value.recievedQuantity,
    statusId: value.statusId,
  };
};
/**Funcion para parsear OrderDetallebyId aOrderDetalle */
export function orderDetailbyIdParser(value: any) {
  return {
    productId: value.productName,
    id: value.id,
    orderedQuantity: value.orderedQuantity,
    recievedQuantity: value.recievedQuantity,
    statusId: value.statusId,
  };
}
