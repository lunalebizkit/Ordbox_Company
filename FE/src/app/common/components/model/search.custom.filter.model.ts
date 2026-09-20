import { formatDate } from "@angular/common";
import { FormGroup } from "@angular/forms";
import { isNil } from "ng-zorro-antd/core/util";

export interface SearchCustomFilterModel {
   filter: {
      supplier: string,
      category: string,
      statusid: number | null,
      number: number | null,
      cuit: string,
      date: string,
      customerName: string
    },
    page: number,
    pageSize: number 
}

export const initialSearchFilter: SearchCustomFilterModel = {
  filter: {
    supplier: "",
    category: "",
    statusid: null,
    number: null,
    cuit: "",
    date: "",
    customerName: ""
  },
  page: 0,
  pageSize: 20
};

function formaterDate(date: string | number | Date, locale: string): string {
    if (!isNil(date))
    return formatDate(date, 'YYYY-MM-dd', locale);
  else{return '';}
}

export function  parseFilterCustomSeachData(queryParams: SearchCustomFilterModel, customSearchForm: FormGroup, locale: string): SearchCustomFilterModel {
    queryParams.filter.customerName = customSearchForm.controls['customerName'].value;
    queryParams.filter.number = customSearchForm.controls['invoicenumber'].value;
    queryParams.filter.cuit = customSearchForm.controls['cuit'].value;
    queryParams.filter.date = formaterDate(customSearchForm.controls['date'].value, locale);
    return queryParams;
}

export function resetQuerySearchFilter(): SearchCustomFilterModel {
  return {
    filter: { ...initialSearchFilter.filter },
    page: initialSearchFilter.page,
    pageSize: initialSearchFilter.pageSize
  };
}
