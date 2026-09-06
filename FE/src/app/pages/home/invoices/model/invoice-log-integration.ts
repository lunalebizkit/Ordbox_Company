export interface InvoiceLog {
    id: number,
    invoiceId: number,
    request: string,
    response: string,
    endPoint: string,
    success: boolean,
    createdOn: Date
}