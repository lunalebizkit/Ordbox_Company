export interface ProductReport{
    date: Date,
    total: number,
    products: Products[]
}
 export interface Products{
    brandName: string,
    code: string,
    description: string,
    id: number,
    quantity: number,
    purchase_Price: number,
    subTotal: number
}