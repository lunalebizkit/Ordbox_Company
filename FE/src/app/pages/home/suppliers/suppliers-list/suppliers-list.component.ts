import { Component, ElementRef, OnInit, ViewChild, signal } from '@angular/core';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { CustomerModel } from '../../customers/model/customer.model';
import { EntityService } from '../../customers/customer.service';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { SearchFilterComponent } from '../../../../common/components/search-filter/search.filter.component';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { CuitPipe } from '../../../../common/pipes/cuit.pipe';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-suppliers-list',
  templateUrl: './suppliers-list.component.html',
  styleUrls: ['./suppliers-list.component.css'],
  imports: [PopupConfirmationComponent, NzLayoutModule, NzPageHeaderModule, NzIconModule, PermissionDirective, SearchFilterComponent, NzTableModule, NzSpaceModule, CuitPipe, NzButtonModule, RouterLink]
})
export class SuppliersListComponent extends BaseComponent implements OnInit {
  selectedIndex: number = 0;
  selectedSuppliers: any;
  permissions = Permission;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  /*
   ** Catidad total de entidades
   */
  totalItems = signal<number>(0);
  /*
   ** Indicador de carga de la grilla
   */
  loading= signal<boolean>(false);
  /*
   ** Lista de Productos
   */
  entityList= signal<CustomerModel[]>([]);
  id = signal<number>(0);
  index!: number;
  /*
   ** Parametros de busqueda
   */
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  /*
   ** Constructor
   */
  constructor(
    private service: EntityService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private router: Router,

  ) {super( notificacionService, el, message)}
  /*
   ** Evento de inicio de angular
   */
  ngOnInit(): void {}
  /*
   ** Evento al presionar buscar o presionar enter
   */
  search(): void {
    this.queryParams.page = 0;
    this.getData(this.queryParams);
  }
  /*
   ** Evento de busqueda datos en el server
   */

  getData(params: any): void {
    this.loading.set(true);
    this.service.getSuppliers(params).subscribe({
      next: (r) => {
        this.entityList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.entityList.set([]);
      },
    });
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/suppliers/edit/${id}`]);
  }

  handleOk() {
    this.service.deleteCustomer(this.popupComponent.elementSelectedToDelete() ?? 0).subscribe(
     {next: (r) => {
        this.popupComponent.isDeleteConfirmationVisible.set(false);
        this.showMessageSuccess("Entidad eliminada");
        this.search();
      },
      error:(r) => { 
        this.showMessageError(r.error.descripcion);
        this.popupComponent.isDeleteConfirmationVisible.set(false);
      }
  });
  }

  onQueryParamsChange(event: NzTableQueryParams){
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }
}
