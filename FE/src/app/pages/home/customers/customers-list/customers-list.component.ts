import {
  signal,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { EntityService } from '../customer.service';
import { CustomerModel } from '../model/customer.model';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { SearchFilterComponent } from '../../../../common/components/search-filter/search.filter.component';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { CuitPipe } from '../../../../common/pipes/cuit.pipe';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-customers-list',
  templateUrl: './customers-list.component.html',
  styleUrls: ['./customers-list.component.css'],
  imports: [NzLayoutModule, NzPageHeaderModule, PermissionDirective, SearchFilterComponent, NzTableModule, NzSpaceModule, PopupConfirmationComponent, NzIconModule, CuitPipe, NzButtonModule, RouterLink]
})

export class CustomersListComponent extends BaseComponent implements OnInit {
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  permissions = Permission;
  /*
   ** Catidad total de entidades
   */
  totalItems= signal<number>(0);
  /*
   ** Indicador de carga de la grilla
   */
  loading = signal<boolean>(false);
  /*
   ** Lista de Productos
   */
  entityList= signal<CustomerModel[]>([]);
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
    private router: Router
  ) { super(notificacionService, el, message) }
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

  getData(params: any): void {
    this.loading.set(true);
    this.service.getCustomers(params).subscribe({
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

  handleOk() {
    this.service.deleteCustomer(this.popupComponent.elementSelectedToDelete() ?? 0).subscribe(
      {
        next: (r) => {
          this.popupComponent.isDeleteConfirmationVisible.set(false);
          this.showMessageSuccess("Entidad eliminada");
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
    this.getData(this.queryParams);
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/invoices/edit/${id}`]);
  }
}
