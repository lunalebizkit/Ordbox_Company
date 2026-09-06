import { Component, ElementRef, OnInit, signal, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { BrandsService } from '../brands.services';
import { BrandsModel } from '../model/brands.model';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { SearchFilterComponent } from '../../../../common/components/search-filter/search.filter.component';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-brands-list',
  templateUrl: './brands-list.component.html',
  styleUrls: ['./brands-list.component.css'],
  imports: [NzLayoutModule, SearchFilterComponent, NzTableModule, PermissionDirective, NzSpaceModule, PopupConfirmationComponent, NzIconModule, NzPageHeaderModule, NzButtonModule, RouterModule]
})
export class BrandsListComponent extends BaseComponent implements OnInit {

  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  permissions = Permission;

  brandList = signal<BrandsModel[]>([]);
  totalItems = signal<number>(0);
  loading = signal<boolean>(false);
  isSaving = signal<boolean>(false);

  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  constructor(
    private service: BrandsService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private router: Router,
  ) { super(notificacionService, el, message) }

  ngOnInit(): void {
  }

  getBrand(params: any): void {
    this.loading.set(true);

    this.service.getByFilter(params).subscribe({
      next: (r) => {
        this.brandList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.brandList.set([]);
      },
    });
  }

  search(): void {
    this.queryParams.page = 0;
    this.getBrand(this.queryParams);
  }
  /*
   ** Evento que selecciona una fila en la tabla.
   */
  onDoubleClicked(id: number) {
    this.router.navigate([`/home/brands/edit/${id}`]);
  }

  handleOk() {
    this.service.delete(this.popupComponent.elementSelectedToDelete() ?? 0).subscribe(
      {
        next: (r) => {
          this.popupComponent.isDeleteConfirmationVisible.set(false);
          this.showMessageSuccess("Marca eliminada");
          this.search();
        },
        error: (r) => {
          this.showMessageError(r.error.descripcion);
          this.popupComponent.isDeleteConfirmationVisible.set(false);
        }
      });
  }

  onQueryParamsChange(event: NzTableQueryParams){
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getBrand(this.queryParams);
  }
}