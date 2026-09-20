export interface ProductAddModel {
   
    id: number,
    description: string,
    code: string,
    categoryid: number,
    brandid: number,
    quantity: number,
    purchasePrice: number,
    salePrice: number,
    salePercentage: number,
    cardSalePrice: number,
    cardSalePercentage: number,
    cashSalePrice: number,
    cashSalePercentage: number,
    pointOrder: number,
    observation: string,
    supplierid: number,
    barCode: string
}
export interface Image {
    uid: string;
    isNew: boolean;
    name: string;
  }