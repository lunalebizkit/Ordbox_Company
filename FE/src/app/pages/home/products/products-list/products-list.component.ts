import { formatCurrency } from '@angular/common';
import { Component, ElementRef, HostListener, Inject, LOCALE_ID, OnInit, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ProductsModel } from '../model/product.model';
import { ProductService } from '../product.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BrandsService } from '../../brands/brands.services';
import { BrandsModel } from '../../brands/model/brands.model';
import { NzModalModule, NzModalService, NzModalState } from 'ng-zorro-antd/modal';
import { ProductCodeBarModal } from '../products-barcode-modal/products-barcode-modal.component';
import { isNil } from 'ng-zorro-antd/core/util';
import { EntityService } from '../../customers/customer.service';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { CategoriesService } from '../../categories/category.services';
import { CategoryModel } from '../../categories/model/category.model';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.css'],
  imports: [NzTableModule, NzLayoutModule, NzPageHeaderModule, PermissionDirective, NzCollapseModule, NzFormModule, NzSelectModule, NzInputModule, ReactiveFormsModule, NzModalModule, NzFormModule, NzButtonModule, NzIconModule, RouterModule]
})
export class ProductsListComponent extends BaseComponent implements OnInit {
  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.key === 'F4' && !this.isDrawerOpen) {
      this.createComponentModal();
    }
  }
  permissions = Permission;
  /*
   ** Listado de los productos
   */
  productList = signal<ProductsModel[]>([]);
  categorieList = signal<CategoryModel[]>([]);
  brandList = signal<BrandsModel[]>([]);
  allSuppliers: { value: string, label: string }[] = [];
  supplierSelected = signal<[]>([]);
  
  clickId!: number;
  formSearch!: FormGroup;
  timeout!: any;
  /*
   ** Indicador de carga de la grilla
   */
  loading = signal<boolean>(false);
  isDrawerOpen = signal<boolean>(false);
  searchInactiveProduct = signal<boolean>(false);
  /*
   ** Catidad total de productos
   */
  totalItems = signal<number>(0);

  /*
   ** Lista de marcas
   */
  brandsList = signal<[]>([]);

  /*
   ** Lista de Productos
   */
  productLinesList = signal<[]>([]);

  /*
   ** Indicador de carga de marcas y lineas
   */
  loadingBrands = signal<boolean>(false);
  isLoading = signal<boolean>(false);

  /*
  ** Parametros de busqueda
  */
  queryParams = {
    filter: {
      product: '',
      brand: 0,
      code: '',
      barCode: '',
      category: 0,
      status: 0,
      supplier: []
    },
    page: 0,
    pageSize: 20
  };

  queryData = {
    filter: '',
    page: 0,
    pageSize: 20
  };

  formProductsEditComponent: any;

  /*
   ** Constructor
   */
  constructor(
    private service: ProductService,
    private serviceCategory: CategoriesService,
    private serviceBrand: BrandsService,
    private serviceEntity: EntityService,
    @Inject(LOCALE_ID) public locale: string,
    notificacionService: NzNotificationService,
    private fb: FormBuilder,
    el: ElementRef,
    message: NzMessageService,
    private modalService: NzModalService,
    private router: Router,
  ) {
    super(notificacionService, el, message);
    this.formSearch = this.fb.group({
      product: ['',],
      code: ['',],
      brand: [0,],
      supplier: [[],],
      category: [0,]
    })
  }

  selectedIndex!: number;
  selectedProduct: any;
  productId!: number | null;
  index!: number;

  /*
   ** Evento de inicio de angular
   */
  ngOnInit(): void {
    this.formSearch.get('product')?.valueChanges.subscribe(value => {
      this.queryParams.filter.product = value;
    });

    this.formSearch.get('code')?.valueChanges.subscribe(val => this.queryParams.filter.code = val);

  }
  /*
   ** Evento de busqueda datos en el servers
   */

  getData(params: any): void {
    this.loading.set(true);
    this.service.getProducts(params).subscribe({
      next: (r) => {
        this.productList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.productList.set([]);
      },
    });
  }

  getInactiveData(params: any): void {
    this.loading.set(true);
    this.service.getInactivesProducts(params).subscribe({
      next: (r) => {
        this.productList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
        this.selectedIndex = 0;
      },
      error: () => {
        this.loading.set(false);
        this.productList.set([]);
      },
    });
  }
  /*
   ** Evento al presionar buscar o presionar enter
   */

  search(): void {
    this.queryParams.page = 0;
    this.searchInactiveProduct.set(false);
    this.getData(this.queryParams);
  }
  searchInactive(): void {
    this.queryParams.page = 0;
    this.searchInactiveProduct.set(true);
    this.getInactiveData(this.queryParams);
  }

  onClick(datos: ProductsModel, index: number): void {
    this.index = index;
    this.selectedIndex = index;
    this.selectedProduct = datos;
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
  }
  //Busca por marca
  onSearchBrand(data: string): void {
    if (data.length > 0) {
      this.queryData.page = 0;
      this.queryData.filter = data;
      this.getBrand(this.queryData);
    }
  }

  getBrand(params: any): void {
    this.loadingBrands.set(true);
    this.serviceBrand.getByFilter(params).subscribe({
      next: (r) => {
        this.brandList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loadingBrands.set(false);
      },
      error: () => {
        this.loadingBrands.set(false);
        this.brandList.set([]);
      },
    });
  }

  brandSelectedChange(id: any): void {
    this.queryParams.filter.brand = id;

  }

  categorySelectedChange(id: any): void {
    this.queryParams.filter.category = id;
  }

  //Busca por categoria
  onSearchCategory(data: string): void {
    if (data.length > 0) {
      this.queryData.page = 0;
      this.queryData.filter = data;
      this.getCategory(this.queryData);
    }
  }

  getCategory(params: any): void {
    this.loading.set(true);
    this.serviceCategory.getByFilter(params).subscribe({
      next: (r) => {
        this.categorieList.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.categorieList.set([])
      },
    });
  }

  createComponentModal(): void {
    const modal = this.modalService.create({
      nzTitle: 'Código de Barra',
      nzContent: ProductCodeBarModal
    });

    // Return a result when closed
    modal.afterClose.subscribe({
      next: (data: string) => {
        this.queryParams.filter.barCode = '';
        if (!isNil(data) && (data)) {
          this.queryParams.filter.barCode = data;
        }
        this.getData(this.queryParams);
      },
      error: e => { console.log(e); }
    })
  }

  haveFilterData(): boolean {
    return (this.queryParams.filter.barCode != '' || this.queryParams.filter.brand > 0 || this.queryParams.filter.category > 0 || this.queryParams.filter.code != '' || this.queryParams.filter.product != '' || this.queryParams.filter.supplier.length > 0);
  }

  clearQueryAndSearch(): void {
    let newFilter = {
      product: '',
      brand: 0,
      code: '',
      barCode: '',
      category: 0,
      status: 0,
      supplier: []
    };

    this.queryParams.filter = newFilter;
    this.formSearch.controls['brand'].setValue(0);
    this.formSearch.controls['category'].setValue(0);

    this.getData(this.queryParams);
  };

  getAllSupplier(): void {
    this.serviceEntity.getSuppliers(this.queryData).subscribe({
      next: (r) => {
        this.allSuppliers = r.data.map((entity: { id: any, name: any }) => { return { value: entity.id, label: entity.name } });
        this.isLoading.set(false);
      },
      error: () => {
        this.allSuppliers = []
      }
    })
  };
  /*
** Evento de busqueda datos en el server
*/
  onSearch(value: string): void {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(() => {

      if (value.length > 0) {
        this.allSuppliers = [];
        this.queryData.filter = value;
        this.getAllSupplier();
      }
    }, 1000);
  };

  supplierSelectedChange(id: any): void {
    this.queryParams.filter.supplier = this.formSearch.controls['supplier'].value;

  }

  onQueryParamsChange(event: NzTableQueryParams) {
    this.queryParams.pageSize = event.pageSize;
    this.queryParams.page = event.pageIndex - 1;
    this.getData(this.queryParams);
  }

  onDoubleClicked(id: number) {
    this.router.navigate([`/home/products/edit/${id}`]);
  }
}
