import { formatDate } from '@angular/common';
import {
  Component,
  ElementRef,
  Inject,
  LOCALE_ID,
  OnInit,
  signal,

} from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CategoriesService } from '../../categories/category.services';
import { EntityService } from '../../customers/customer.service';
import { eStatus, StatusType } from '../models/status-type.enum';
import { NewOrder, NewOrderDetail} from '../models/order.model';
import { OrdersService } from '../orders.service';
import { CustomerModel } from '../../customers/model/customer.model';
import { CategoryModel } from '../../categories/model/category.model';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { Router, RouterModule } from '@angular/router';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { ProductFilter, resetProductFilter } from '../../../../common/components/model/product.filter.model';

@Component({
  selector: 'app-orders-list',
  templateUrl: './orders-list.component.html',
  styleUrls: ['./orders-list.component.css'],
  imports: [NzLayoutModule, NzPageHeaderModule, PermissionDirective, RouterModule, NzCollapseModule, ReactiveFormsModule, NzFormModule, NzSelectModule, NzDatePickerModule, NzTableModule, NzButtonModule, NzIconModule, NzInputModule, FormatDatePipe]
})
export class OrdersListComponent extends BaseComponent implements OnInit {
  permissions = Permission;
  formSearch!: FormGroup;
  formSupplierSearch!: FormGroup;
  isLoading = signal<boolean>(false);
  timeout!: any;
  allCategories = signal<any[]>([]);
  allSuppliers: { value: string; label: string }[] = [];
  allOrders = signal<NewOrder[]>([]);
  orderDetailList = signal<NewOrderDetail[]>([]);
  allStatus = StatusType;
  /*
   ** id del usuario a editar, si es nuevo...
   */
  loading = signal<boolean>(false);
  totalItems=signal<number>(0);

  /*
   ** Parametros de busqueda Filtrada
   */
  queryParams: ProductFilter = resetProductFilter;
  /*
   ** Parametros de busqueda
   */
  queryData = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  entityList = signal<CustomerModel[]>([]);
  categorieList: CategoryModel[] = [];
  selectedOrders!: NewOrder;
  index!: number;
  editId!: number;

  constructor(
    private serviceOrders: OrdersService,
    private serviceCategory: CategoriesService,
    private serviceEntity: EntityService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    private router: Router,
    @Inject(LOCALE_ID) public locale: string
  ) {
    super(notificacionService, el, message);
    this.formSearch = this.fb.group({
      status: [1],
      supplier: [[]],
      category: [0],
      date: ['']
    })
    this.formSupplierSearch = this.fb.group({
      supplierId: ['', [Validators.required]],
    });
  }
  ngOnInit(): void {
  }
  /*
   ** Indicador de carga de marcas y lineas
   */
  loadingBrands!: boolean;
  isLoadingCategory = signal<boolean>(false);
  isLoadingBrand = false;
  isLoadingEntity = false;

  getAllOrders(queryParams: any): void {
    this.isLoading.set(true);
    this.serviceOrders.getOrders(queryParams).subscribe({
      next: (r) => {
        this.isLoading.set(false);
        this.allOrders.set(r.data);
        this.totalItems.set(r.totalCount);
      },
      error: () => {
        this.isLoading.set(false)
        this.allOrders.set([]);
      },
    });
  }

  /*
   ** Evento de busqueda datos en el server
   */
  onSearch(data: string): void {
    if (data.length > 2) {
      this.queryData.page = 0;
      this.queryData.filter = data;
      this.getSupplier(this.queryData);
    }
  }

  getSupplier(params: any): void {
    this.loading.set(true);
    this.serviceEntity.getSuppliers(params).subscribe({
      next: (r) => {
        this.entityList.set(r.data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.entityList.set([]);
      },
    });
  }

  //Busca por categoria
  onSearchCategory(data: string): void {
    if (data.length > 2) {
      this.queryData.page = 0;
      this.queryData.filter = data;
      this.getCategory(this.queryData);
    }
  }

  getCategory(params: any): void {
    this.isLoadingCategory.set(true);
    this.serviceCategory.getByFilter(params).subscribe({
      next: (r) => {
        this.isLoadingCategory.set(false);
        this.allCategories.set(r.data.map(
          (category: { id: any; description: any }) => {
            return { value: category.id, label: category.description };
          }
        ));
      },
      error: () => {
        this.isLoadingCategory.set(false);
        this.allCategories.set([]);
      },
    });
  }

  supplierSelectedChange(id: any): void {

    if (id == 0 || id == null) {
      this.queryParams.filter.supplier = [0];
    } else {
      this.queryParams.filter.supplier = [id];
    }
  }

  dateSelectedChange(date: Date): void {    
    this.queryParams.filter.date = date;
  }

  categorySelectedChange(id: number): void {
    this.queryParams.filter.category = id;
  }

  statusSelectedChange(id: number): void {
    this.queryParams.filter.status = id;
  }  

  /*
   ** Evento al presionar buscar o presionar enter
   */
  search(): void {
    this.queryParams.page = 0;
    this.orderDetailList.set([]);
    this.getAllOrders(this.queryParams);
  }

  getStatusName(id: number) {
    return eStatus[id];
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/orders/edit/${id}`]);
  }  
    
  onClick(id: number): void {
    this.orderDetailList.set(this.allOrders().filter(
      (order) => order.id == id
    )[0].orderDetail);
  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getAllOrders(this.queryParams);
  }
}
