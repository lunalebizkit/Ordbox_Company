import { Pipe, PipeTransform } from '@angular/core';
import { eInvoiceType } from '../../pages/home/invoices/model/invoice-type.Enum';

@Pipe({name: 'invoiceTypeFilter'})
export class InvoiceTypePipe implements PipeTransform{
    transform(value: eInvoiceType):string {
        if (!value) return '';

        if (value == eInvoiceType.EXENTO) return 'B';
        if (value == eInvoiceType.RespMonotributo) return 'A';
    
    return eInvoiceType[value];
    }
}