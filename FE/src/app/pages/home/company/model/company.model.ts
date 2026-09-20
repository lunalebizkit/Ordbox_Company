import { signal } from "@angular/core";


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
  companyConcept: number,
  companyConditionIva: number,
  isDeleted: boolean,
}

export interface CompanyCertificateModel {
  companyId: number,
  password: string,
  certificateData: File | null
}

export enum ECondicionIva {
  ResponsableInscripto = 1,
  IvaSujetoExento = 4,
  ConsumidorFinal = 5,
  ResponsableMonotributo = 6,
  SujetoNoCategorizado = 7,
  ProveedorDelExterior = 8,
  ClienteDelExterior = 9,
  IVALiberado = 10,
  MonotributistaSocial = 13,
  IVANoAlcanzado = 15,
}

function enumToOptions<T>(enumObj: any): { value: number, label: string }[] {
  return Object.keys(enumObj)
    .filter(k => !isNaN(Number(enumObj[k])))
    .map(k => ({
      value: enumObj[k] as number,
      label: k.replace(/([A-Z])/g, ' $1').trim()
    }));
}

export const condicionIvaList = signal(enumToOptions(ECondicionIva))