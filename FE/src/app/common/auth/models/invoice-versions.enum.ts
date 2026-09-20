import { signal } from "@angular/core";

export enum InvoiceVersion{
    Default = 0,
    Arca = 1
}

export enum EConcepto {
  Productos = 1,
  Servicios = 2,
  ProductosYServicios = 3
}

function enumToOptions<T>(enumObj: any): { value: number, label: string }[] {
  return Object.keys(enumObj)
    .filter(k => !isNaN(Number(enumObj[k])))
    .map(k => ({
      value: enumObj[k] as number,
      label: k.replace(/([A-Z])/g, ' $1').trim()
    }));
}

export const concepList = signal(enumToOptions(EConcepto))