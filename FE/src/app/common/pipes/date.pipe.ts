import { Pipe, PipeTransform } from '@angular/core';
import { formatDate } from '@angular/common';

@Pipe({
  name: 'datePipe',
  standalone: true
})
export class FormatDatePipe implements PipeTransform {
  transform(value: string | number | Date, locale: string = 'es-AR'): string {
    if (!value) return '';
    return formatDate(value, 'yyyy-MM-dd HH:mm', locale);
  }
}