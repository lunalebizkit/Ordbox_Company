import { formatCurrency, formatDate } from '@angular/common';
import { Component, ElementRef, Inject, Input, LOCALE_ID, OnInit, signal, ViewChild } from '@angular/core';
import { InvoiceService } from '../invoices.service';
import { receiptListModel } from '../model/receipt.model';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { parseFilterCustomSeachData, resetQuerySearchFilter, SearchCustomFilterModel } from '../../../../common/components/model/search.custom.filter.model';
import { SearchCustomFilterComponent } from '../../../../common/components/search-custom-filter/search.custom.filter.component';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { CuitPipe } from '../../../../common/pipes/cuit.pipe';
import { NoCommaPipe } from '../../../../common/pipes/no-comma.pipe';
import { InvoiceTypePipe } from '../../../../common/pipes/invoice-type.pipe';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-receipt-list',
  templateUrl: './receipt-list.component.html',
  styleUrls: ['./receipt-list.component.css'],
  imports: [SearchCustomFilterComponent, NzLayoutModule, NzPageHeaderModule, NzTableModule, PermissionDirective, NzSpaceModule, CuitPipe, NoCommaPipe, InvoiceTypePipe, NzIconModule, NzButtonModule, PopupConfirmationComponent, FormatDatePipe, RouterModule]
})
export class ReceiptListComponent extends BaseComponent implements OnInit {
  permissions = Permission;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  customRSearchForm!: FormGroup;
  /*
   ** Indicador de carga de la grilla
   */
  loading = signal<boolean>(false);
  totalItems = signal<number>(0);

  /*
   ** Parametros de busqueda
   */
  queryParams: SearchCustomFilterModel = resetQuerySearchFilter();

  receiptList = signal<receiptListModel[]>([]);

  constructor(
    @Inject(LOCALE_ID) public locale: string,
    private service: InvoiceService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    private router: Router
  ) {
    super(notificacionService, el, message);
    this.customRSearchForm = this.fb.group({
      cuit: [''],
      customerName: [''],
      invoicenumber: [],
      date: [null]
    })
  }

  ngOnInit(): void { }

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
    let loadedparams = parseFilterCustomSeachData(params, this.customRSearchForm, this.locale);
    this.service.getReceipt(loadedparams).subscribe({
      next: (r) => {
        this.receiptList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.receiptList.set([]);
      },
    });
  }

  currencyFormat(data: any): string {
    if (!this.locale) return '';
    return formatCurrency(data, this.locale!, '$', 'ARS', '1.1-2');
  }

  onDoubleClick(id: number) {
     this.router.navigate([`/home/receipts/edit/${id}`]);
  }

  reimprimirReceipt(id: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `Comprobante_de_Compra_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.ReprintReceipt(id).subscribe({
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

  handleOk() {
    this.service.delete(this.popupComponent.elementSelectedToDelete() ?? 0).subscribe(
      {
        next: (r) => {
          this.popupComponent.isDeleteConfirmationVisible.set(false);
          this.showMessageSuccess("Comprobante eliminado");
          this.search();
        },
        error: (r) => {
          this.showMessageError(r.error.descripcion);
          this.popupComponent.isDeleteConfirmationVisible.set(false);
        }
      });
  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }
  
}
