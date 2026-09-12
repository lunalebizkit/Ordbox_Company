

export interface CompanyListModel {
  id:number;
  name: string;
  ownerName: string;
}

export interface CompanyModel {
  id: number,
  companyName: string,
  companyOwnerName: string,
  companyCuit: string,
  companyAddress: string,
  companyEmail: string,
  companyEmailPass: string,
  companyDescription: string,
  companyPoint: number,
  isDeleted: boolean,
}

export interface CompanyCertificateModel {
  companyId: number,
  password: string,
  certificateData: File | null
}