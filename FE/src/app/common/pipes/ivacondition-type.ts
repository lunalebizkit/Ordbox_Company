import { Pipe, PipeTransform } from '@angular/core';
import { eInvoiceType } from '../../pages/home/invoices/model/invoice-type.Enum';

@Pipe({name: 'ivaConditionTypeFilter'})
export class IvaConditionTypePipe implements PipeTransform{
    transform(value: eInvoiceType):string {
        switch(value){
            case eInvoiceType.A:
                return 'Resp. Inscripto';
            case eInvoiceType.RespMonotributo:
                return 'Resp. Monotributo';
            case eInvoiceType.B:
                return 'Consumidor final';
            case eInvoiceType.EXENTO:
                return 'Exento';
            default:
                return '';
        }
    }
}