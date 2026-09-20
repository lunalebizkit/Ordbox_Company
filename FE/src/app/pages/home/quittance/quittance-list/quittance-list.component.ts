import { Component, OnInit, Inject, LOCALE_ID, signal } from '@angular/core';
import { formatCurrency, formatDate } from '@angular/common';
import { QuittanceService } from '../quittance.service';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { quittanceModel } from '../model/model';
import { parseFilterCustomSeachData, resetQuerySearchFilter, SearchCustomFilterModel } from '../../../../common/components/model/search.custom.filter.model';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { SearchCustomFilterComponent } from '../../../../common/components/search-custom-filter/search.custom.filter.component';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NoCommaPipe } from '../../../../common/pipes/no-comma.pipe';
import { CuitPipe } from '../../../../common/pipes/cuit.pipe';

@Component({
  selector: 'app-quittance-list',
  templateUrl: './quittance-list.component.html',
  styleUrls: ['./quittance-list.component.css'],
  imports: [NzLayoutModule, NzPageHeaderModule, PermissionDirective, SearchCustomFilterComponent, NzTableModule, NzIconModule, NzSpaceModule, RouterModule, FormatDatePipe, NzButtonModule, NzInputModule, NzCollapseModule, NoCommaPipe, CuitPipe]
})
export class QuittanceListComponent implements OnInit {
  form!: FormGroup;
  permissions = Permission;
  loading = signal<boolean>(false);
  totalItems = signal<number>(0);
  dataList = signal<quittanceModel[]>([]);

  /*
   ** Parametros de busqueda
   */
  queryParams: SearchCustomFilterModel = resetQuerySearchFilter();

  /*
   ** Constructor
   */
  constructor(
    private service: QuittanceService,
    private router: Router,
    @Inject(LOCALE_ID) public locale: string,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      cuit: [''],
      customerName: [''],
      invoicenumber: [0],
      date: [null]
    })
  }

  /*
   ** Evento de inicio de angular
   */
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
    let loadedparams = parseFilterCustomSeachData(params, this.form, this.locale);
    this.service.getQuittance(loadedparams).subscribe({
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

  reimprimirQuittance(id: number): void {
    let fecha: Date = new Date();
    let año: string = fecha.getFullYear().toString();
    let mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    let dia = fecha.getDate().toString().padStart(2, '0');
    let hora: string = fecha.getHours().toString().padStart(2, '0');
    let minutos: string = fecha.getMinutes().toString().padStart(2, '0');
    let segundos: string = fecha.getSeconds().toString().padStart(2, '0');
    const fileName = `Recibo_${año}${mes}${dia}${hora}${minutos}${segundos}`;
    this.service.ReprintQuittance(id).subscribe({
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
    this.router.navigate(['/home/quittances/edit/', id]);
  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }

}




