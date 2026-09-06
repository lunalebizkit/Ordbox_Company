import { Component, OnInit, Inject, LOCALE_ID, signal } from '@angular/core';
import { InvoiceListModel } from '../model/invoice.model';
import { formatCurrency, formatDate } from '@angular/common';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { InvoiceVersion } from '../../../../common/auth/models/invoice-versions.enum';
import { InvoiceService } from '../invoices.service';
import { parseFilterCustomSeachData, resetQuerySearchFilter, SearchCustomFilterModel } from '../../../../common/components/model/search.custom.filter.model';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { SearchCustomFilterComponent } from '../../../../common/components/search-custom-filter/search.custom.filter.component';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { CuitPipe } from '../../../../common/pipes/cuit.pipe';
import { InvoiceTypePipe } from '../../../../common/pipes/invoice-type.pipe';
import { Router, RouterModule } from '@angular/router';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';

@Component({
  selector: 'app-invoices-list',
  templateUrl: './invoices-list.component.html',
  styleUrls: ['./invoices-list.component.css'],
  imports: [NzLayoutModule, NzPageHeaderModule, PermissionDirective, NzTableModule, NzCollapseModule, SearchCustomFilterComponent, NzButtonModule, NzIconModule, CuitPipe, InvoiceTypePipe, RouterModule, FormatDatePipe, RouterModule]
})
export class InvoicesListComponent implements OnInit {

  customSearchForm!: FormGroup;
  permissions = Permission;
  invoiceVersion = InvoiceVersion;
  /*
   ** Catidad total de entidades
   */

  totalItems = signal<number>(0);
  /*
   ** Indicador de carga de la grilla
   */
  loading = signal<boolean>(false);
  /*
   ** Lista de Productos
   */
  invoicesList = signal<InvoiceListModel[]>([]);
  /*
   ** Parametros de busqueda
   */
  queryParams: SearchCustomFilterModel = resetQuerySearchFilter();

  /*
   ** Constructor
   */
  constructor(
    private service: InvoiceService,
    @Inject(LOCALE_ID) public locale: string,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.customSearchForm = this.fb.group({
      cuit: [''],
      customerName: [''],
      invoicenumber: [0],
      date: [null]
    })
  }

  /*
   ** Evento de inicio de angular
   */
  ngOnInit(): void {
  }
  /*
   ** Evento al presionar buscar o presionar enter
   */
  search(): void {
    this.getData(this.queryParams);
    this.queryParams.page = 0;
    this.queryParams.pageSize = 20;

  }
  /*
   ** Evento de busqueda datos en el server
   */

  getData(params: any): void {
    this.loading.set(true);
    let loadedparams = parseFilterCustomSeachData(params, this.customSearchForm, this.locale);
    this.service.getInvoices(loadedparams).subscribe({
      next: (r) => {
        this.invoicesList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.invoicesList.set([]);
      },
    });
  }

  formaterDate(date: string | number | Date): string {
    return formatDate(date, 'YYYY-MM-dd hh:mm', this.locale);
  }

  currencyFormat(data: any): string {
    if (!this.locale) return '';
    return formatCurrency(data, this.locale!, '$', 'ARS', '1.1-2');
  }

  reimprimirInvoice(id: number, invoiceNumber: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `Factura_Proforma_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.Reprintinvoice(id).subscribe({
      next: (r) => { this.downloadFile(r, fileName); }

    });
  }

  downloadFile(response: any, fileName: string) {
    const dataType = response.type;
    const binaryData = [];

    binaryData.push(response);

    const filtePath = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }))
    const downloadLink = document.createElement('a');
    downloadLink.href = filtePath;
    downloadLink.setAttribute('download', fileName);
    document.body.appendChild(downloadLink);
    downloadLink.click();
  }

  imprimirInvoiceArca(id: number, invoiceNumber: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `Factura_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.printInvoiceARCA(id).subscribe({
      next: (r) => { this.downloadFile(r, fileName); }

    });
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/invoices/edit/${id}`]);
  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }

  onToCreditClicked(id: number) {
    this.router.navigate(
      ['/home/credits/edit', id],
      { state: { source: 'invoices' } }
    );
  }

  onToDebitClicked(id: number) {
    this.router.navigate(['/home/debits/edit/', id], { state: { source: 'invoices' } });
  }
}
