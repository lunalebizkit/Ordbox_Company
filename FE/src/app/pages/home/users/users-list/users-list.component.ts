import { Component, ElementRef, OnInit, ViewChild, signal } from '@angular/core';
import { UserService } from '../users.services';
import { ListUserModel } from '../model/list.user.model';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { eRol } from '../model/rol.enum';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { SearchFilterComponent } from '../../../../common/components/search-filter/search.filter.component';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { ActivatedRoute, Router, RouterLink, RouterModule } from "@angular/router";

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css'],
  imports: [NzPageHeaderModule, NzLayoutModule, NzTableModule, NzIconModule, NzSpaceModule, PopupConfirmationComponent, SearchFilterComponent, PermissionDirective, NzButtonModule, RouterLink, RouterModule]
})
export class UsersListComponent extends BaseComponent implements OnInit {  
  
  permissions = Permission;
  userRol!: string | null;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  /*
   ** Listado de los usuarios
   */
  userList = signal<ListUserModel[]>([]);
  /*
   ** Indicador de carga de la grilla
   */
  totalItems = signal<number>(0);
  loading = signal<boolean>(false)
  isLoadingRoles = true;

  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  constructor(
    private service: UserService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private router: Router,
    private route: ActivatedRoute,
  ) { super(notificacionService, el, message) }
  selectedIndex: number = 0;
  selectedUser: any;

  ngOnInit(): void {
    this.userRol = localStorage.getItem('auth-user');
  }

  /*

  /*
 ** Evento de busqueda datos en el server
 */
  getData(params: any): void {
    this.loading.set(true);
    this.service.getByFilter(params).subscribe({
      next: (r) => {
        this.userList.set(r.data);

        // Envía el numero total de páginas
        this.totalItems.set(r.totalCount);

        // Saca spinner de carga
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.userList.set([]);
      },
    });
  }
  /*
   ** Evento al presionar buscar o presionar enter
   */

  search(): void {
    this.queryParams.page = 0;
    this.getData(this.queryParams);
  }

  getRolName(id: number) {
    return eRol[id];
  }  

  handleOk() {
    this.service.deleteUser(this.popupComponent.elementSelectedToDelete() ?? 0).subscribe(
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

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/users/edit/${id}`]);
  }
}
