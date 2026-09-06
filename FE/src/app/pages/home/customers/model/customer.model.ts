export interface CustomerModel {
    id: number,
    dni: string,
    cuit: string,
    name: string,
    address: string,
    observation: string,
    phoneEntity: String,
    emailEntity:string,
    email: string | null
}