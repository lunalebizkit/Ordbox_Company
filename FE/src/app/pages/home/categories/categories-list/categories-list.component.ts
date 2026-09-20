import { Component, ElementRef, OnInit, signal, ViewChild } from '@angular/core';
import { CategoriesService } from '../category.services';
import { CategoryModel } from '../model/category.model';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { SearchFilterComponent } from '../../../../common/components/search-filter/search.filter.component';
import { NzLayoutComponent, NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-categories-list',
  templateUrl: './categories-list.component.html',
  styleUrls: ['./categories-list.component.css'],
  imports : [PopupConfirmationComponent, NzSpaceModule, NzIconModule, NzButtonModule, PermissionDirective, NzTableModule, SearchFilterComponent, NzLayoutComponent, NzPageHeaderModule, NzLayoutModule, RouterModule]
})
export class CategoriesListComponent extends BaseComponent implements OnInit {
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  permissions = Permission;
  /*
   ** Indicador de carga de la grilla
   */
  loading = signal<boolean>(false);
  /*
   ** Catidad total de Categorias
   */
  totalItems = signal<number>(0);

  categoryList= signal<CategoryModel[]>([]);
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  constructor(
    private service: CategoriesService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private router: Router,
  ) { super( notificacionService, el, message)}

  ngOnInit(): void {
  }
  /*
   ** Evento de busqueda datos en el server
   */
  getData(params: any): void {
    this.loading.set(true);
    this.service.getByFilter(params).subscribe({
      next: (r) => {
        this.categoryList.set(r.data);
        // Envía el numero total de páginas
        this.totalItems.set(r.totalCount);
        // Saca spinner de carga
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.categoryList.set([]);
      },
    });
  }
  
  search(): void {
    this.queryParams.page = 0;
    this.getData(this.queryParams);
  }
  /*
   ** Evento de selección de filas en la tabla
   */


  handleOk() {
    this.service.delete(this.popupComponent.elementSelectedToDelete() ?? 0).subscribe(
     {next: (r) => {
        this.popupComponent.isDeleteConfirmationVisible.set(false);
        this.showMessageSuccess("Categoría eliminada");
        this.search();
      },
      error:(r) => { 
        this.showMessageError(r.error.descripcion);
        this.popupComponent.isDeleteConfirmationVisible.set(false);
      }
  });
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/categories/edit/${id}`]);
  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }
}
