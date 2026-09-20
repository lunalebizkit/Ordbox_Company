import { Component, ElementRef, Inject, LOCALE_ID, OnInit, signal, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { BudgetsService } from '../budgets.services';
import { BudgetsModel } from '../model/budgets.model';
import { formatCurrency, formatDate } from '@angular/common';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { parseFilterCustomSeachData, resetQuerySearchFilter, SearchCustomFilterModel } from '../../../../common/components/model/search.custom.filter.model';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { SearchCustomFilterComponent } from '../../../../common/components/search-custom-filter/search.custom.filter.component';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';

@Component({
  selector: 'app-budgets-list',
  templateUrl: './budgets-list.component.html',
  styleUrls: ['./budgets-list.component.css'],
  imports: [NzLayoutModule, NzPageHeaderModule, PermissionDirective, SearchCustomFilterComponent, NzTableModule, PopupConfirmationComponent, NzIconModule, NzSpaceModule, RouterModule, FormatDatePipe, NzButtonModule, NzInputModule]
})

export class BudgetsListComponent extends BaseComponent implements OnInit {

  customBSearchForm!: FormGroup;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  permissions = Permission;
  loading = signal<boolean>(false);
  totalItems = signal<number>(0);
  dataList = signal<BudgetsModel[]>([]);
  /*
   ** Parametros de busqueda
   */
  queryParams: SearchCustomFilterModel = resetQuerySearchFilter();

  constructor(
    private service: BudgetsService,
    private router: Router,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    @Inject(LOCALE_ID) public locale: string,
  ) {
    super(notificacionService, el, message);
    this.customBSearchForm = this.fb.group({
      cuit: [''],
      customerName: [''],
      invoicenumber: [0],
      date: [null]
    })
  }

  search(): void {
    this.getBudget(this.queryParams);
    this.queryParams.page = 0;
    this.queryParams.pageSize = 20;
  }

  ngOnInit(): void {
  }

  getBudget(params: any): void {
    this.loading.set(true);
    let loadedparams = parseFilterCustomSeachData(params, this.customBSearchForm, this.locale);
    this.service.getByFilter(loadedparams).subscribe({
      next: (r) => {
        this.dataList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.dataList.set([]);
      },
    });
  }

  onDoubleClick(id: number) {
    this.router.navigate(['/home/budgets/edit/', id]);
  }

  print(id: Number) {
    window.open('_/' + id, "_blank");
  }

  formaterDate(date: string | number | Date): string {
    return formatDate(date, 'YYYY-MM-dd', this.locale);
  }

  currencyFormat(data: any): string {
    if (!this.locale) return '';
    return formatCurrency(data, this.locale!, '$', 'ARS', '1.1-2');
  }

  reimprimirBudgets(id: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `Presupuesto_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.ReprintBudgets(id).subscribe({
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
          this.showMessageSuccess("Presupuesto eliminado");
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
    this.getBudget(this.queryParams);
  }
}