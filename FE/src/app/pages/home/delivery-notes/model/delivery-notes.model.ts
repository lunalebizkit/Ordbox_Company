export interface DeliveryNotesModel {
  id: number;
  deliveryNotes_number: number;
  dateTime: Date;
  supplierId: number;
  supplierName: string;
  supplierCuit: string;
  supplierAddress: string;
  statusId: number;
  cancelled: string;
  paid: boolean;
  observation: string;
  importTotal: number;
  deliveryNotesDetails: DeliveryNotesDetails[]

}
export interface DeliveryNotesDetails {
  id: number;
  deliveryNotesId: number;
  productId: number;
  productName: string;
  quantity: number;
  price: number
}
export interface DeliveryNotesGrid {
  productId: number;
  productName: string;
  ownCode: number;
  quantity: number;
  price: number;
  subTotal: number
}

export function deliveryNotesGridParser(value: any, price: number) {
  return {
    productId: value.id,
    productName: value.description,
    price: value.purchasePrice,
    quantity: 1,
    subTotal: price,
    ownCode: value.id,
  };
}

export function deliveryNotesGridFromParser(value: any, index: number) {
  return {
    productId: (value.productId == -1) ? -(index + 1) : value.productId,
    productName: value.productName,
    ownCode: (value.productId == -1) ? -(index + 1) : value.productId,
    price: value.price,
    quantity: value.quantity,
    subTotal: value.price * value.quantity
  }
}

export function deliveryNotesDetailFromParser(value: any, index: number) {
  return {
    id: value.id,
    deliveryNotesId: value.id,
    productId: (value.productId == -1) ? -(index + 1) : value.productId,
    productName: value.productName,
    ownCode: value.id,
    price: value.price,
    quantity: value.quantity,
  }
}

export function deliveryNotesDetailParser(value: any, price: number) {
  return {
    id: 0,
    deliveryNotesId: 0,
    productId: value.id,
    productName: value.description,
    price: value.purchasePrice,
    quantity: 1,
  };
}