import { Component, computed, ElementRef, Input, OnInit, signal, TemplateRef, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { NzDrawerModule, NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { EntityService } from "../../customers/customer.service";
import { ActivatedRoute, Router } from "@angular/router";
import { formatCurrency } from '@angular/common';
import { Inject, LOCALE_ID } from '@angular/core';
import { QuittanceService } from "../quittance.service";
import { InvoiceCustomerSearchComponent } from "../../invoices/invoice-customer-search/invoice-customer-search.component";
import { CustomerModel } from "../../customers/model/customer.model";
import { isNil } from "ng-zorro-antd/core/util";
import { ProductService } from "../../products/product.service";
import { ProductsModel } from "../../products/model/product.model";
import { InvoiceProductSearchComponent } from "../../invoices/invoice-product-search/invoice-product-search.component";
import { IvaType } from "../../invoices/model/iva-type.Enum";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { quittanceDetailParser, QuittanceDetails, QuittanceGrid, quittanceGridParser, QuittanceProductDetails, quittancesDetailFromParser, quittancesGridFromParser } from "../model/model";
import { Permission } from "../../../../common/auth/models/permissions.enum";
import { AuthService } from "../../../../common/auth/interceptors/auth.service";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzDividerModule } from "ng-zorro-antd/divider";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzInputModule } from "ng-zorro-antd/input";
import { NzInputNumberModule } from "ng-zorro-antd/input-number";
import { NzLayoutModule } from "ng-zorro-antd/layout";
import { NzPageHeaderModule } from "ng-zorro-antd/page-header";
import { NzSelectModule } from "ng-zorro-antd/select";
import { NzSpaceModule } from "ng-zorro-antd/space";
import { NzSwitchModule } from "ng-zorro-antd/switch";
import { NzTableModule } from "ng-zorro-antd/table";
import { NzTagModule } from "ng-zorro-antd/tag";
import { AppCommonModule } from "../../../../common/app.common.module";
import { NzDatePickerModule } from "ng-zorro-antd/date-picker";


@Component({
  selector: 'app-quittance-edit',
  templateUrl: './quittance-edit.component.html',
  styleUrls: ['./quittance-edit.component.css'],
  imports: [HeaderOperationsButtonsComponent, FormsModule, NzFormModule, ReactiveFormsModule, NzCollapseModule, NzLayoutModule, NzDatePickerModule, NzTagModule, NzPageHeaderModule, NzSelectModule, NzDividerModule, NzSwitchModule, AppCommonModule, NzTableModule, PopupConfirmationComponent, NzInputNumberModule, NzInputModule, NzSpaceModule, NzDrawerModule]
})
export class QuittanceEditComponent extends BaseComponent implements OnInit {
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  @ViewChild('drawerTemplate', { static: false }) drawerTemplate?: TemplateRef<{
    $implicit: { filter: string },
    drawerRef: NzDrawerRef<string>;
  }>;

  loading = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  form: FormGroup;

  startDate = Date.now();
  formCustomerSearch!: FormGroup;
  formProductSearch!: FormGroup;
  formProduct!: FormGroup;
  formQuittanceDetails!: FormGroup;
  cuit!: string;
  name!: string;
  address!: string;
  bank!: string;
  checkNumber!: string;
  dateTime!: Date;
  product= signal<string>('');
  iva: number = 21;
  id = signal<number>(0);
  userId = signal<number>(0);
  permissions = Permission;
  editProductId: number = 0;
  editId = signal<number | null>(null);
  editIdIva = signal<number | null>(null);
  editIdProductPrice = signal<number | null>(null);
  editIdProductName = signal<number | null>(null);
  customerId = signal<number>(0);
  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(false);
  ivaType = IvaType;

  dataDetails = signal<QuittanceProductDetails[]>([]);
  dataGrid = signal<QuittanceGrid[]>([]);

  subtotal = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.quantity * item.price, 0)
  );

  total = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.price * item.quantity, 0)
  );

  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  constructor(notificacionService: NzNotificationService,
    private serviceEntity: EntityService,
    private service: QuittanceService,
    private router: Router,
    private route: ActivatedRoute,
    el: ElementRef,
    message: NzMessageService,
    private drawerService: NzDrawerService,
    private fb: FormBuilder,
    private serviceProduct: ProductService,
    public serviceUser: AuthService,
    @Inject(LOCALE_ID) public locale: string) {
    super(notificacionService, el, message);
    this.form = this.fb.group({
      dateTime: [{value: new Date(this.startDate), disabled: true}, Validators.required],
      quittanceNumber: [{ value: 0, disabled: true }, Validators.required],
      address: [{ value: '', disabled: true },],
      customerCuit: [{ value: '', disabled: true }, [Validators.required, Validators.pattern('^[0-9]{8,11}$'),]],
      customerName: [{ value: '', disabled: true }, Validators.required],
      concept: [{ value: '', disabled: true },],
      quittanceDetails: new FormArray([])
    });

    this.formCustomerSearch = this.fb.group({});

    this.formProductSearch = this.fb.group({ productSearchFilter: [{ value: '', disabled: true }] })
  }

  get quittanceDetailsFormGroups(): FormArray {
    return this.form.get('quittanceDetails') as FormArray
  }

  ngOnInit() {
    this.userId.set(this.serviceUser.currentUser()?.id ?? 0);
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.getQuittance(p['id']);
        } else {
          this.isEditMode.set(true);
          this.isInvoiceEditable.set(true);
          this.form.enable();
          this.formProductSearch.enable();
        }
      },
      error: () => { }
    })
  }

  getQuittance(id: number): void {
    if (id != 0) {
      this.isLoading.set(true);
      this.id.set(id);
      this.service.getById(id).subscribe({
        next: (r) => {
          this.form.patchValue({
            dateTime: new Date(r.dateTime),
            quittanceNumber: r.id,
            address: r.address,
            customerCuit: r.customerCuit,
            customerName: r.customerName,
            concept: r.concept
          });
          this.id.set(r.id);
          this.dataGrid.set(r.quittanceProductDetails.map((modelDetail: QuittanceGrid, index: number) => {
            return quittancesGridFromParser(modelDetail, index)
          }));

          this.dataDetails.set(r.quittanceProductDetails.map((modelDetail: QuittanceDetails, index: number) => {
            return quittancesDetailFromParser(modelDetail, index)
          }));

          this.dateTime = r.dateTime
          r.quittanceDetails.map((data: any, key: any) => {
            this.showPasswordChangeBox(data, key)
          })

          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  currencyFormat(data: any): string {
    if (data != null || data != undefined) {
      return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2');
    } else return ''
  }

  showPasswordChangeBox(data?: any, key?: any): void {
    let newQuittancegroup = this.fb.group({
      checkNumber: [''],
      bank: [''],
      total: [''],

    })

    this.quittanceDetailsFormGroups.push(newQuittancegroup);
    if (data != undefined) {
      this.quittanceDetailsFormGroups.controls[key].get('checkNumber')?.setValue(data.checkNumber)
      this.quittanceDetailsFormGroups.controls[key].get('bank')?.setValue(data.bank)
      this.quittanceDetailsFormGroups.controls[key].get('total')?.setValue(data.total)
    }

  }

  removeCheck(e: MouseEvent, index: any): void {

    e.preventDefault();
    this.quittanceDetailsFormGroups.removeAt(index)
  };

  save(): void {
    if (this.isValidForm(this.form)) {
      //EDITAR
      if (this.id() > 0) {
        const model = this.form.getRawValue();
        model.id = this.id();
        model.quittanceProductDetails = this.dataDetails();
        this.isSaving.set(true);
        this.service.editQuittance(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Recibo editado correctamente`
              );
              this.isSaving.set(false);
              this.router.navigate(['/home/quittances']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error)
            }
          });
        //GUARDAR
      } else {
        const model = this.form.getRawValue();
        model.id = this.id();
        model.quittanceProductDetails = this.dataDetails();
        this.isSaving.set(true);
        this.service.saveQuittance(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Recibo creado correctamente`
              );
              this.isSaving.set(false);
              this.router.navigate(['/home/quittances']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error)
            }
          });
      }
    }

  }

  searchCustomer(): void {
    this.cuit =
      this.form.controls['customerCuit'].value;
    if (this.cuit == '00') {
      this.form.controls['address'].setValue('S/D');
      this.form.controls['customerCuit'].setValue('99999999995');
      this.form.controls['customerName'].setValue('Admin');
      this.customerId.set(0);
      return;
    } else {
      if (this.cuit.length >= 6) {
        this.serviceEntity.getByCuit(this.cuit).subscribe({
          next: (data) => {
            this.form.controls['address'].setValue(data.address);
            this.form.controls['customerCuit'].setValue(data.cuit);
            this.form.controls['customerName'].setValue(data.name);
          },
          error: () => { this.showMessageError('No se encontro Cliente'); }
        });
      }
    }
  };

  openComponentCustomer(): void {
    const drawerRefCustomer = this.drawerService.create<InvoiceCustomerSearchComponent, {}, CustomerModel>({
      nzTitle: 'Cliente',
      nzContent: InvoiceCustomerSearchComponent,
      nzSize: 'large',
      nzWidth: '90%',
      nzClosable: false
    });
    drawerRefCustomer.afterClose.subscribe({
      next: (data) => {
        if (data != undefined) {
          this.customerId.set(data.id);
          this.form.controls['address'].setValue(data.address);
          this.form.controls['customerCuit'].setValue(!isNil(data.cuit) ? data.cuit.replace(/[^a-zA-Z0-9 ]/g, '') : null);
          this.form.controls['customerName'].setValue(data.name);
        }
      },
      error: () => { }
    })
  };

  msjConfirmOk() {
    try {

      this.dataGrid.set(this.dataGrid().filter(element => element.ownCode != this.popupComponent.elementSelected()));

      this.popupComponent.isConfirmationvisible.set(false);

      if (this.dataGrid().length === 0) {
        this.showMessageError('No ha seleccionado producto');
        return;
      }

      if (!this.isValidProductName()) {
        this.showMessageError('Hay productos sin descripción');
        return;
      }

      if (this.isValidForm(this.form) && this.isValidForm(this.formProductSearch)) {
        this.popComponent.showConfirmation()
      }
    } catch (error) { console.log(error); }
  }

  searchProduct(): void {
    this.product.set(this.formProductSearch.controls['productSearchFilter'].value);

    if (this.product() === '00') {
      this.addNewEditProduct();
      return;
    }

    if (!this.product() || this.product().length === 0) {
      this.isLoading.set(false);
      return;
    }

    const productParams = {
      filter: {
        product: this.product(),
        code: '',
        barCode: '',
        brand: 0,
        category: 0,
        status: 0,
        supplier: [] as Number[]
      },
      page: 0,
      pageSize: 50
    };

    this.serviceProduct.getProducts(productParams).subscribe({
      next: (r) => {
        this.isLoading.set(true);
        if (r.data.length === 1) {
          const product: ProductsModel = r.data[0];
          this.addOrUpdateProduct(product);
        }
        else {
          this.isLoading.set(false);
          this.openComponentProduct();
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.formProductSearch.controls['productSearchFilter'].setValue('');
      }
    })
  }

  addNewEditProduct(): void {
    this.editProductId--;
    let newEditProduct: ProductsModel = {
      id: this.editProductId,
      quantity: 1,
      code: '',
      description: '',
      cashSalePrice: 0,
      categoryName: '',
      brandName: '',
      purchasePrice: 0,
      salePrice: 0,
      salePercentage: 0,
      cardSalePrice: 0,
      cashSalePercentage: 0,
      cardSalePercentage: 0,
      pointOrder: 0,
      observation: '',
      supplierName: '',
      isDeleted: false,
      barCode: ''
    };
    /* Parseo el Producto a la grilla de Tabla */
    const model: QuittanceGrid = quittanceGridParser(newEditProduct, this.iva);
    this.dataGrid.update(item => [...item, model]);

    /* Parseo dato a Dto Factura Detalle */
    const modelDetail: QuittanceProductDetails = quittanceDetailParser(newEditProduct, this.iva);
    this.dataDetails.update(item => [...item, modelDetail]);

    this.isLoading.set(false);

    this.formProductSearch.controls['productSearchFilter'].setValue('');
  }

  openComponentProduct(): void {
    if (this.formProductSearch.controls['productSearchFilter'].value == '00') {
      this.addNewEditProduct();
      return;
    }
    let drawerRefProduct = this.drawerService.create<InvoiceProductSearchComponent, { filter: string }, [ProductsModel]>({
      nzTitle: 'Productos',
      nzContent: InvoiceProductSearchComponent,
      nzSize: 'large',
      nzWidth: '90%',
      nzContentParams: {
        filter: this.formProductSearch.controls['productSearchFilter'].value
      },
      nzClosable: false
    });
    drawerRefProduct.afterClose.subscribe({
      next: (data: [ProductsModel] | undefined) => {
        if (data != undefined) {
          data.forEach((productItem) => {
            this.addOrUpdateProduct(productItem);
          })
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.formProductSearch.controls['productSearchFilter'].setValue('');
      }
    });
  };

  startEdit(id: number): void {
    this.editId.set(id);
  }

  stopEdit(): void {
    this.editId.set(null);
  }

  startEditIva(id: number): void {
    this.editIdIva.set(id);
  }

  startEditProductName(id: number): void {
    this.editIdProductName.set(id);
  }

  stopEditIva(): void {
    this.editIdIva.set(null);
  }
  
  stopEditProductName(): void {
    this.editIdProductName.set(null);
  }

  startEditProductPrice(id: number): void {
    this.editIdProductPrice.set(id);
  }

  stopEditProductPrice(): void {
    this.editIdProductPrice.set(null);
  }

  changeQuantity(quantity: number): void {
    if (!quantity || quantity <= 0) quantity = 1;

    this.dataGrid.update(list =>
      list.map(detail =>
        detail.ownCode === this.editId()
          ? { ...detail, quantity, subTotal: quantity * detail.price }
          : detail
      )
    );

    this.dataDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editId()
          ? { ...detail, quantity }
          : detail
      )
    );
  }

  changeProductName(name: string): void {
    this.dataGrid.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );

    this.dataDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );
  }

  changeProductPrice(price: number): void {
    this.dataGrid.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductPrice()
          ? { ...detail, price, subTotal: detail.quantity * price }
          : detail
      )
    );

    this.dataDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductPrice()
          ? { ...detail, price }
          : detail
      )
    );
  }

  handleOk() {
    try {
      const idToDelete = this.popupComponent.elementSelectedToDelete();

      const updatedGrid = this.dataGrid().filter(el => el.ownCode !== idToDelete);
      const updatedDetails = this.dataDetails().filter(el => el.productId !== idToDelete);

      this.dataGrid.set(updatedGrid);
      this.dataDetails.set(updatedDetails);

      this.popupComponent.isDeleteConfirmationVisible.set(false);
    } catch (error) { console.log(error); }
  };

  isValidProductName(): boolean {
    return this.dataDetails().every(y => typeof y.productName === "string" && y.productName.trim() !== "")
  }

  changeIvaValue(iva: number, id: number): void {
    let newIva = Number(iva);
    try {
      this.dataGrid.update(list =>
        list.map(detail =>
          detail.productId === id
            ? { ...detail, iva: newIva }
            : detail
        )
      );

      this.dataDetails.update(list =>
        list.map(detail =>
          detail.productId === id
            ? { ...detail, iva: newIva }
            : detail
        )
      );

      this.stopEditIva();

    } catch (error) {
      console.error(error);
    }
  }

  addOrUpdateProduct(product: ProductsModel): void {
    const existingDetail = this.dataDetails().find(item => item.productId === product.id);

    if (existingDetail) {
      // actualizar cantidad y subtotal
      existingDetail.quantity += 1;

      const gridDetail = this.dataGrid().find(item => item.ownCode === product.id);
      if (gridDetail) {
        gridDetail.quantity += 1;
        gridDetail.subTotal = product.cashSalePrice * product.quantity;
      }
    } else {
      // agregar nuevo producto
      const gridDetail: QuittanceGrid = quittanceGridParser(product, this.iva)
      const detail: QuittanceProductDetails = quittanceDetailParser(product, this.iva);

      this.dataGrid.update(list => [...list, gridDetail]);
      this.dataDetails.update(list => [...list, detail]);
    }
  }

  toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
      this.form.enable();
      this.formProductSearch.enable();
      this.isInvoiceEditable.set(true);
    } else {
      this.form.disable();
      this.formProductSearch.disable();
      this.isInvoiceEditable.set(false);
    }
  }
}