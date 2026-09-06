import { Component, Inject, LOCALE_ID, OnInit, signal, ViewChild } from '@angular/core';
import { NzDrawerService } from 'ng-zorro-antd/drawer';
import { formatCurrency } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { SearchCustomFilterComponent } from '../../../../common/components/search-custom-filter/search.custom.filter.component';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { parseFilterCustomSeachData, resetQuerySearchFilter, SearchCustomFilterModel } from '../../../../common/components/model/search.custom.filter.model';
import { DeliveryNotesModel } from '../model/delivery-notes.model';
import { deliveryNotesService } from '../delivery-notes.service';
import { pStatusType } from '../model/status.model';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NoCommaPipe } from '../../../../common/pipes/no-comma.pipe';

@Component({
  selector: 'app-delivery-notes-list',
  templateUrl: './delivery-notes-list.component.html',
  styleUrls: ['./delivery-notes-list.component.css'],
  imports: [NzLayoutModule, NzPageHeaderModule, PermissionDirective, SearchCustomFilterComponent, NzTableModule, PopupConfirmationComponent, NzIconModule, NzSpaceModule, RouterModule, FormatDatePipe, NzButtonModule, NzInputModule, NzCollapseModule, NoCommaPipe]
})
export class DeliveryNotesListComponent implements OnInit {

  form!: FormGroup;
  permissions = Permission;
  loading = signal<boolean>(false);
  totalItems = signal<number>(0);
  dataList = signal<DeliveryNotesModel[]>([]);
  /*
   ** Parametros de busqueda
   */
  queryParams: SearchCustomFilterModel = resetQuerySearchFilter();

  constructor(
    private service: deliveryNotesService,
    @Inject(LOCALE_ID) public locale: string,
    private router: Router,
    private fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      cuit: [''],
      customerName: [''],
      invoicenumber: [0],
      date: [null]
    })
  }

  ngOnInit(): void { }

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
    let loadedparams = parseFilterCustomSeachData(params, this.form, this.locale);
    this.service.getDeliveryNotes(loadedparams).subscribe({
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

  currencyFormat(data: any): string {
    if (!this.locale) return '';
    return formatCurrency(data, this.locale!, '$', 'ARS', '1.1-2');
  }

  getStatusName(id: number) {
    return pStatusType[id];
  }

  print(id: Number) {
    window.open('__/' + id, "_blank");
  }

  reimprimirdeliveryNotes(id: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `Remito_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.ReprintdeliveryNotes(id).subscribe({
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
  
  onDoubleClick(id: number) {
    this.router.navigate(['/home/delivery-notes/edit/', id]);
  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }
}

