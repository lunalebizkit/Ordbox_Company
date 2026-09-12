import { formatCurrency } from '@angular/common';
import { Component, ElementRef, HostListener, Inject, Input, LOCALE_ID, OnInit, signal, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { BrandsService } from '../../brands/brands.services';
import { CategoriesService } from '../../categories/category.services';
import { EntityService } from '../../customers/customer.service';
import { ProductAddModel } from '../model/product.add.model';
import { ProductService } from '../product.service';
import { ProductCodeBarModal } from '../products-barcode-modal/products-barcode-modal.component';
import { NzModalService } from 'ng-zorro-antd/modal';
import { isNil } from 'ng-zorro-antd/core/util';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { HeaderOperationsButtonsComponent } from '../../../../common/components/headers/buttons.oparations.header.component';
import { AuthService } from '../../../../common/auth/interceptors/auth.service';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { ButtonOperationFooter } from '../../../../common/components/footers/button.operation.footer.component';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { PermissionDirective } from '../../../../common/directives/permission.directive';
import { ActivatedRoute, Route } from '@angular/router';

@Component({
  selector: 'app-products-edit',
  templateUrl: './products-edit.component.html',
  imports: [NzPageHeaderModule, HeaderOperationsButtonsComponent, NzSpinModule, NzCollapseModule, ReactiveFormsModule, NzLayoutModule, NzFormModule, NzSelectModule, NzInputModule, ButtonOperationFooter, PopupConfirmationComponent, NzInputNumberModule, PermissionDirective]
})
export class ProductsEditComponent extends BaseComponent implements OnInit {

  @Input() codeBar!: string;
  permissions = Permission;
  @ViewChild('popupDelete') popupDeleteComponent!: PopupConfirmationComponent;
  @ViewChild('popupActive') popupActiveComponent!: PopupConfirmationComponent;
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;
  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.key === 'F4') {
      this.createComponentModal();
    }
  }
  /*
 ** Determina si esta en proceso de guardado
 */
  isSaving= signal<boolean>(false);
  queryData = {
    filter: '',
    page: 0,
    pageSize: 100,
  };


  /*
   ** Determina si esta buscando el usuario
   */
  isLoading = signal<boolean>(false);
  isLoadingCategory = signal<boolean>(false);
  isLoadingBrand = signal<boolean>(false);
  isLoadingEntity = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  categorySelected: any = null;
  brandSelected: any = null;
  entitySelected: any = null;
  timeout!: any;
  /*
   ** id del usuario a editar, si es nuevo...
   */
  id = signal<number>(0);
  isDeleted!: boolean;
  /*
** Formulario
*/
  form!: FormGroup;
  /**
 * Url del la imagen en preview
 */
  previewImage: string | undefined = '';

  /**
   * Determina si esta o no el preview activo
   */
  previewVisible = false;

  /**
   * Lista de imagenes
   */
  imagesList: NzUploadFile[] = [];

  allCategories: { value: string, label: string }[] = [];
  allBrands: { value: string, label: string }[] = [];
  allSuppliers: { value: string, label: string }[] = [];
  constructor(
    private service: ProductService,
    private serviceCategory: CategoriesService,
    private serviceBrand: BrandsService,
    private serviceEntity: EntityService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    @Inject(LOCALE_ID) public locale: string,
    private modalService: NzModalService,
    private permissionService: AuthService
  ) {
    super(notificacionService, el, message);
    this.form = this.fb.group({
      description: [{value:'', disabled: true}, [Validators.required]],
      code: [{value:'', disabled: true}],
      categoryName: [{value:'', disabled: true}, [Validators.required]],
      brandName: [{value:'', disabled: true}, [Validators.required]],
      salePercentage: [{value:50, disabled: true}, [Validators.required]],
      cardSalePercentage: [{value:60, disabled: true}, [Validators.required]],
      cashSalePercentage: [{value:40, disabled: true}, [Validators.required]],
      salePrice: [{value:'', disabled: true}, [Validators.required]],
      purchasePrice: [{value:0, disabled: true}, [Validators.required]],
      cashSalePrice: [{value:0, disabled: true}, [Validators.required]],
      cardSalePrice: [{value:0, disabled: true}, [Validators.required]],
      supplierName: [{value:'', disabled: true}, [Validators.required]],
      quantity: [{value:0, disabled: true}, [Validators.required]],
      pointOrder: [{value:0, disabled: true}, [Validators.required]],
      observation: [{value:'', disabled: true}, []],
      barCode: [{value:'', disabled: true}, []]

    })
  }

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.isLoading.set(true);
          this.getProduct(p['id']);
        }
      },
      error: () => { }
    });
  };
  /*
** Evento de busqueda datos en el server
*/
  onSearch(value: string): void {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(() => {

      if (value.length > 2) {
        this.allSuppliers = [];
        this.queryData.filter = value;
        this.getAllSupplier();
      }
    }, 1000);
  };

  onSearchBrand(value: string): void {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(() => {

      if (value.length > 2) {
        this.allBrands = [];
        this.queryData.filter = value;
        this.getAllBrands();
      }
    }, 1000);
  };

  onSearchCategory(value: string): void {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(() => {

      if (value.length > 2) {
        this.allCategories = [];
        this.queryData.filter = value;
        this.getAllCategories();
      }
    }, 1000);
  };

  getProduct(id: number): void {
    if (id != 0) {
      this.id.set(id);
      this.service.getById(id).subscribe({
        next: (r) => {
          this.isDeleted = r.isDeleted;
          this.allCategories = r.category;
          this.allBrands = r.brand;
          this.allSuppliers = r.supplier;
          Object.keys(this.form.controls).forEach((key: string) => {
            const ctr = this.form.controls[key];
            const value = r[key]
            if (value !== undefined && value !== null) {
              switch (key) {
                case "categoryName":
                  if (r.category != null && r.category[0].value != null)
                    ctr.setValue(r.category[0].value);
                  break;

                case "supplierName":
                  if (r.supplier != null && r.supplier[0].value != null)
                    ctr.setValue(r.supplier[0].value);
                  break;

                case "brandName":
                  if (r.brand != null && r.brand[0].value != null)
                    ctr.setValue(r.brand[0].value);
                  break;
                default:
                  ctr.setValue(value)
              }


            }
          });
          this.isLoading.set(false);
        },
        error: () => { this.isLoading.set(false); }
      })
    }
  }

  entitySelectedChange(id: any): void {
    this.queryData.filter = id != undefined ? id : this.form.controls['supplierName'].value;

  }

  categorySelectedChange(id: any): void {
    this.queryData.filter = id != undefined ? id : this.form.controls['categoryName'].value;
  }
  brandSelectedChange(id: any): void {
    this.queryData.filter = id != undefined ? id : this.form.controls['brandName'].value;
  }
  save(): void {
    if (this.isValidForm(this.form)) {
      const model: ProductAddModel = {
        id: this.id(),
        description: this.form.controls['description'].value,
        code: this.form.controls['code'].value,
        categoryid: this.form.controls['categoryName'].value,
        brandid: this.form.controls['brandName'].value,
        cashSalePrice: this.form.controls['cashSalePrice'].value,
        cashSalePercentage: this.form.controls['cashSalePercentage'].value,
        quantity: this.form.controls['quantity'].value,
        purchasePrice: this.form.controls['purchasePrice'].value,
        salePrice: this.form.controls['salePrice'].value,
        salePercentage: this.form.controls['salePercentage'].value,
        cardSalePrice: this.form.controls['cardSalePrice'].value,
        cardSalePercentage: this.form.controls['cardSalePercentage'].value,
        pointOrder: this.form.controls['pointOrder'].value,
        observation: this.form.controls['observation'].value,
        supplierid: this.form.controls['supplierName'].value,
        barCode: this.form.controls['barCode'].value
      };
      this.isSaving.set(true);
      this.service.saveProduct(model).subscribe({
        next: (r) => {
          this.showNotificationSuccess(
            'Guardado correcto',
            `Se guardo correctamente el Producto ${model.description}`
          );
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
          this.showMessageError('No se pudo Guardar el Producto');
        }
      })
    }
  }

  getAllCategories(): void {
    this.isLoadingCategory.set(true);
    this.serviceCategory.getByFilter(this.queryData).subscribe({
      next: (r) => {
        this.isLoadingCategory.set(false);
        this.allCategories = r.data.map((category: { id: any, description: any }) => { return { value: category.id, label: category.description } });

      },
      error: () => {
        this.isLoadingCategory.set(false);
        this.allCategories = []
      }
    })
  }
  getAllBrands(): void {
    this.isLoadingBrand.set(true);
    this.serviceBrand.getByFilter(this.queryData).subscribe({
      next: (r) => {
        this.isLoadingBrand.set(false);
        this.allBrands = r.data.map((brand: { id: any, description: any }) => { return { value: brand.id, label: brand.description } });
      },
      error: () => {
        this.isLoadingBrand.set(false);
        this.allBrands = []
      }
    })
  }
  getAllSupplier(): void {
    this.isLoadingEntity.set(true);
    this.serviceEntity.getSuppliers(this.queryData).subscribe({
      next: (r) => {
        this.isLoadingEntity.set(false);
        this.allSuppliers = r.data.map((entity: { id: any, name: any }) => { return { value: entity.id, label: entity.name } });
      },
      error: () => {
        this.isLoadingEntity.set(false);
        this.allSuppliers = []
      }
    })
  }
  formatterPeso = (value: number): string => formatCurrency(value, this.locale, '$', 'ARS', '1.1-2');

  formatterPorcentaje = (value: number): string => `${value} %`;

  valuechange(newValue: any) {
    this.onPrecioCosto(this.form.controls['purchasePrice'].value)
  }

  onPrecioCosto(valor: any) {
    this.form.controls['salePrice'].setValue(valor + (valor * this.form.controls['salePercentage'].value / 100))
    this.form.controls['cashSalePrice'].setValue(valor + (valor * this.form.controls['cashSalePercentage'].value / 100))
    this.form.controls['cardSalePrice'].setValue(valor + (valor * this.form.controls['cardSalePercentage'].value / 100))
  }

  msjConfirmOk() {
    try {
      if (this.isValidForm(this.form)) {
        this.popComponent.showConfirmation();
      } else {
        this.showMessageError('Formulario vacio')
      }
    } catch (error) {
      console.log(error);

    }
  }
  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
  }

  createComponentModal(): void {
    const modal = this.modalService.create({
      nzTitle: 'Codigo de Barra',
      nzContent: ProductCodeBarModal
    });

    const instance = modal.getContentComponent();
    // Return a result when closed
    modal.afterClose.subscribe({
      next: (data: string) => {
        if (!isNil(data) && (data))
          this.form.controls['barCode'].setValue(data);
      },
      error: e => { console.log(e); }
    })
  }

  handleOk() {
    this.service.delete(this.id()).subscribe(
      {
        next: (r) => {
          this.popupDeleteComponent.isDeleteConfirmationVisible.set(false);
          this.showMessageSuccess("Producto eliminado");
        },
        error: (r) => {
          this.showMessageError(r.error.descripcion);
          this.popupDeleteComponent.isDeleteConfirmationVisible.set(false);
        }
      });
  }


  handleActiveOk() {
    this.service.activate(this.id()).subscribe(
      {
        next: (r) => {
          this.popupActiveComponent.isConfirmationvisible.set(false);
          this.showMessageSuccess("Producto activado");
        },
        error: (r) => {
          this.showMessageError(r.error.descripcion);
          this.popupActiveComponent.isConfirmationvisible.set(false);
        }
      });
  }

  hasPermission(permissionId: Permission): boolean {
    return this.permissionService.currentUser()?.permission.includes(permissionId) ?? false;
  }
  
  toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
      this.form.enable();
    } else {
      this.form.disable();
    }
  }
}