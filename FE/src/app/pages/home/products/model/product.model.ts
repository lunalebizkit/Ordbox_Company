export interface ProductsModel {
    id: number,
    description: string,
    code: string,
    categoryName: string,
    brandName: string,
    quantity: number,
    purchasePrice: number,
    salePrice: number,
    salePercentage: number,
    cardSalePrice: number,
    cashSalePrice: number,
    cashSalePercentage: number,
    cardSalePercentage: number,
    pointOrder: number,
    observation: string,
    supplierName: string,
    isDeleted: boolean,
    barCode: string
}
export interface Image {
    uid: string;
    isNew: boolean;
    name: string;
  }