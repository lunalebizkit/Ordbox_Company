import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'cuit' })
export class CuitPipe implements PipeTransform {
    
    transform(value: string): string {
        if (!value) return '';
        if (value.length == 11){
        return `${value.substring(0, 2)}-${value.substring(2, 10)}-${value.substring(10, 11)}`;
        }
        return value;
    }
}
