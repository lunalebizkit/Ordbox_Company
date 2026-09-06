import { formatCurrency } from '@angular/common';
import { Component, ElementRef, Inject, LOCALE_ID, OnInit, signal } from '@angular/core';
import { DebitMemoModel } from '../model/debitMemo.model';
import { NoteService } from '../notes.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { InvoiceVersion } from '../../../../common/auth/models/invoice-versions.enum';
import { parseFilterCustomSeachData, resetQuerySearchFilter, SearchCustomFilterModel } from '../../../../common/components/model/search.custom.filter.model';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { SearchCustomFilterComponent } from '../../../../common/components/search-custom-filter/search.custom.filter.component';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { CuitPipe } from '../../../../common/pipes/cuit.pipe';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-debits-list',
  templateUrl: './debits-list.component.html',
  styleUrls: ['./debits-list.component.css'],
  imports: [NzPageHeaderModule, NzTableModule, NzIconModule, NzButtonModule, NzLayoutModule, NzCollapseModule, SearchCustomFilterComponent, NzDividerModule, PermissionDirective, CuitPipe, FormatDatePipe, RouterModule ]
})
export class DebitMemoListComponent extends BaseComponent implements OnInit {

  customDMSearchForm!: FormGroup;
  loading = signal<boolean>(false);
  totalItems = signal<number>(0);
  dataList= signal<DebitMemoModel[]>([]);
  dia: any;
  permissions = Permission;
  invoiceVersion = InvoiceVersion;

  queryParams: SearchCustomFilterModel = resetQuerySearchFilter();

  index!: number;

  constructor(
    private service: NoteService,
    @Inject(LOCALE_ID) public locale: string,
    private fb: FormBuilder,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private router: Router
  ) {
    super(notificacionService, el, message);
    {
      this.customDMSearchForm = this.fb.group({
        cuit: [''],
        customerName: [''],
        invoicenumber: [0],
        date: [null]
      })
    }
  }

  ngOnInit(): void {
  }

  getData(params: any): void {
    this.loading.set(true);
    let loadedparams = parseFilterCustomSeachData(params, this.customDMSearchForm, this.locale);
    this.service.getDebitMemo(loadedparams).subscribe({
      next: (r) => {
        this.dataList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.dataList.set([])
      },
    });
  }

  search(): void {
    this.getData(this.queryParams);
    this.queryParams.page = 0;
    this.queryParams.pageSize = 20;
  }

  onDoubleClick(id: number) {
    this.router.navigate(['/home/debits/edit/', id],
      { state: { source: 'debits' } }
    );
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
  }

  imprimirInvoiceArca(id: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `NotaDebito_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.printDebitARCA(id).subscribe({
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

  onQueryParamsChange(event: NzTableQueryParams) {
      this.queryParams.pageSize = event.pageSize;
      this.queryParams.page = event.pageIndex - 1;
      this.getData(this.queryParams);
    }
}