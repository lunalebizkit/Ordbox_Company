import { Component, ElementRef, OnInit, ViewChild, computed, signal } from "@angular/core";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { CustomerModel } from "../../customers/model/customer.model";
import { EntityService } from "../../customers/customer.service";
import { ProductsModel } from "../../products/model/product.model";
import { InvoiceDetailList, InvoiceDetails, InvoiceModel, invoiceGridParser, invoiceDetailParser, invoiceDetailToGridParser } from "../model/invoice.model";
import { InvoiceService } from "../invoices.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ePayment, paymentTypes } from "../model/invoice-payment.Enum";
import { eInvoiceType, eIvaCondition, InvoiceType, IvaCondition } from "../model/invoice-type.Enum";
import { formatCurrency } from '@angular/common';
import { Inject, LOCALE_ID } from '@angular/core';
import { ProductService } from "../../products/product.service";
import { IvaType } from "../model/iva-type.Enum";
import { forkJoin, map } from "rxjs";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { AuthService } from "../../../../common/auth/interceptors/auth.service";
import { NzPageHeaderModule } from "ng-zorro-antd/page-header";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzInputModule } from "ng-zorro-antd/input";
import { NzLayoutModule } from "ng-zorro-antd/layout";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzSelectModule } from "ng-zorro-antd/select";
import { NzDividerModule } from "ng-zorro-antd/divider";
import { NzTableModule } from "ng-zorro-antd/table";
import { NzInputNumberModule } from "ng-zorro-antd/input-number";
import { NzSpaceModule } from "ng-zorro-antd/space";
import { NzIconModule } from "ng-zorro-antd/icon";
import { NzButtonModule } from "ng-zorro-antd/button";
import { InvoiceVersion } from "../../../../common/auth/models/invoice-versions.enum";
import { Permission } from "../../../../common/auth/models/permissions.enum";
import { FormatDatePipe } from "../../../../common/pipes/date.pipe";
import { NzDrawerModule, NzDrawerService } from "ng-zorro-antd/drawer";
import { InvoiceProductSearchComponent } from "../invoice-product-search/invoice-product-search.component";
import { InvoiceLog } from "../model/invoice-log-integration";
import { XMLParser } from "fast-xml-parser";

@Component({
  selector: 'app-invoices-edit',
  templateUrl: './invoices-edit.component.html',
  styleUrls: ['./invoices-edit.component.css'],
  imports: [NzPageHeaderModule, NzCollapseModule, ReactiveFormsModule, NzInputModule, NzLayoutModule, NzFormModule, NzSelectModule, NzDividerModule, NzTableModule, NzInputNumberModule, NzSpaceModule, NzIconModule, NzButtonModule, PopupConfirmationComponent, FormsModule, HeaderOperationsButtonsComponent, FormatDatePipe, NzDrawerModule]
})
export class InvoicesEditComponent extends BaseComponent implements OnInit {
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  /*
 ** Cantidad total de productos
 */
  invoiceType = InvoiceType;
  ivaCondition = IvaCondition;
  ivaType = IvaType;
  eIvaCondition = eIvaCondition;
  typeSelectedId: number = 1;
  ivaSelected!: number;
  iva10: number = parseFloat('10.5');
  iva21: number = 21;
  iva27: number = 27;
  iva: number = 21;
  subtotal = computed(() =>
    this.invoiceDetailsList().reduce((acc, item) =>
      acc + (item.quantity * this.ivaCalculate(item.price, item.iva)), 0)
  );

  ivaTotal = computed(() =>
    this.invoiceDetailsList().reduce((acc, item) =>
      acc + (item.price - this.ivaCalculate(item.price, item.iva)) * item.quantity, 0)
  );

  total = computed(() =>
    this.invoiceDetailsList().reduce((acc, item) =>
      acc + item.price * item.quantity, 0)
  );

  logLoading = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  loading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  id = signal<number>(0);
  formInvoice!: FormGroup;
  formProductSearch!: FormGroup;
  editProductId: number = 0;
  startDate = Date.now();
  patternCuit: string = '^[0-9]{11}$';
  payment = paymentTypes;

  /*
 ** Lista de Productos
 */
  invoiceDetailsList = signal<InvoiceDetailList[]>([]);
  invoiceDetails = signal<InvoiceDetails[]>([]);
  invoiceLog = signal<InvoiceLog[]>([]);
  version= signal<InvoiceVersion>(0);
  invoiceVersion = InvoiceVersion;

  /*
  **Variables de la tabla detalle
  */
  editId = signal<number | null>(null);
   editIdIva = signal<number | null>(null);
  /*
** Parametros de busqueda
*/
  paymentSelected: any;
  product= signal<string>('');
  customerId!: number;
  invoiceA = signal<boolean>(true);

  /*
** Parametros de busqueda
*/
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 10
  };

  dni: any;
  editIdProductName = signal<number | null>(null);
  editIdProductPrice = signal<number | null>(null);
  entityList = signal<CustomerModel[]>([]);


  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(true);
  permissions = Permission;

  constructor(
    private fb: FormBuilder,
    notificacionService: NzNotificationService,
    private serviceEntity: EntityService,
    private serviceProduct: ProductService,
    private serviceInvoice: InvoiceService,
    public serviceUser: AuthService,
    el: ElementRef,
    private router: Router,
    private drawerService: NzDrawerService,
    private route: ActivatedRoute,
    message: NzMessageService,
    @Inject(LOCALE_ID) public locale: string
  ) {
    super(notificacionService, el, message);
    this.formInvoice = this.fb.group({
      dateTime: [{ value: new Date(this.startDate), disabled: true }, Validators.required],
      type: [{ value: 1, disabled: true }, Validators.required],
      payment: [{ value: eInvoiceType.A, disabled: true }, Validators.required],
      customerAddress: [{ value: '', disabled: true }, Validators.required],
      customerCuit: [{ value: '', disabled: true }, Validators.required],
      customerName: [{ value: '', disabled: true }, Validators.required],
      customerEmail: [{ value: '', disabled: true }, Validators.email],
      observation: [{ value: '', disabled: true }],
      ivaCondition: [{ value: eIvaCondition.RespInscrip, disabled: true }, Validators.required]
    });
    this.formProductSearch = this.fb.group({
      productSearchFilter: [{ value: '', disabled: true }]
    })
  }
  userId = signal<number>(0);

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.isLoading.set(true);
          this.getInvoice(p['id']);
        }
      },
      error: () => { }
    });

    this.paymentSelected = Object.entries(ePayment).find(([key, value]) => value === 'Contado')?.[0];

    this.userId.set(this.serviceUser.currentUser.id);

    this.formInvoice.get('customerCuit')?.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.customerId = 0;
        this.formInvoice.get('customerCuit')?.clearValidators();
      }
    });
    this.formInvoice.get('ivaCondition')?.valueChanges.subscribe(() => this.updatePattern());
  }

  typeSelectedChange(id: eInvoiceType): void {
    this.typeSelectedId = id;

    this.invoiceA.set((id == eInvoiceType.A));

    if (this.invoiceA()) {
      this.ivaCondition = this.ivaCondition.map(opt => ({
        ...opt,
        disabled: (opt.value === 3 || opt.value === 4)
      }));

      this.formInvoice.controls['ivaCondition'].setValue(null);

      // validacion para CUIT
      this.formInvoice.get('customerCuit')?.reset();
      this.formInvoice.get('customerCuit')?.clearValidators();
      this.formInvoice.get('customerCuit')?.updateValueAndValidity();
      this.formInvoice.get('customerCuit')?.setValidators([Validators.required, Validators.pattern(/^\d{11}$/)]);
      this.formInvoice.get('customerCuit')?.updateValueAndValidity();
    }
    else {
      this.ivaCondition = this.ivaCondition.map(opt => ({
        ...opt,
        disabled: (opt.value === 1 || opt.value === 2)
      }));
      this.formInvoice.controls['ivaCondition'].setValue(null);
      // validacion para CUIT
      this.formInvoice.get('customerCuit')?.reset();
      this.formInvoice.get('customerCuit')?.clearValidators();
      this.formInvoice.get('customerCuit')?.updateValueAndValidity();
      this.formInvoice.get('customerCuit')?.setValidators([Validators.required, Validators.pattern(/^\d{11}$/)]);

      this.formInvoice.get('customerCuit')?.updateValueAndValidity();
    }

  }

  paymentSelectedChange(id: any): void {
    let paymentName = this.payment.filter(data => data.value == id)[0].label;
    this.paymentSelected = Object.entries(ePayment).find(([key, value]) => value === paymentName)?.[0];
    this.updatePriceByPaymentSelectedChange();
  }

  searchCustomer(data: string): void {
    this.loading.set(true);
    let cuit = this.clearCuitString(data);
    if (cuit === '00') {
      this.formInvoice.controls['customerAddress'].setValue('-');
      this.formInvoice.controls['customerCuit'].setValue('99999999995');
      this.formInvoice.controls['customerName'].setValue('-');
      this.customerId = 0;
      return;
    } else {
      if (cuit.length >= 3) {
        this.serviceEntity.getCustomersByCuit(cuit).subscribe({
          next: (data) => {
            if (data && data.length > 0) {
              this.entityList.set(data);
              this.loading.set(false);
            } else {
              this.loading.set(false);
              this.entityList.set([]);
            }
          },
          error: () => { this.loading.set(false); this.showMessageError('No se encontro resultado'); }
        });
      }
    }
  };

  bindPrice(data: ProductsModel): number {
    const typePayment = this.paymentSelected;
    var a = Object.keys(data).filter(type => (type == typePayment));
    switch (a[0]) {
      case 'cardSalePrice':
        return data.cardSalePrice;

      case 'salePrice':
        return data.salePrice;

      default:
        return data.cashSalePrice;
    }
  };

  ivaCalculate(data: number, iva: number): number {
    let newIva = 1 + (iva / 100);
    return (data / newIva);
  }

  handleOk(): void {
    try {
      const idToDelete = this.popupComponent.elementSelectedToDelete();

      const updatedList = this.invoiceDetailsList().filter(el => el.ownCode !== idToDelete);
      const updatedDetails = this.invoiceDetails().filter(el => el.productId !== idToDelete);

      this.invoiceDetailsList.set(updatedList);
      this.invoiceDetails.set(updatedDetails);

      this.popupComponent.isDeleteConfirmationVisible.set(false);

    } catch (error) {
      console.error('Error en handleOk:', error);
    }
  }

  msjConfirmOk() {
    try {
      this.invoiceDetailsList.set(this.invoiceDetailsList().
        filter(element => element.ownCode != this.popupComponent.elementSelected()));

      this.popupComponent.isConfirmationvisible.set(false);

      if (this.invoiceDetailsList().length == 0) {
        this.showMessageError('No ha seleccionado producto');
        return;
      }

      if (!this.isValidProductName()) {
        this.showMessageError('Hay productos sin descripción');
        return;
      }

      if (this.isValidForm(this.formInvoice) && this.isValidForm(this.formProductSearch)) {
        this.popComponent.showConfirmation()
      }
    } catch (error) {
      console.log(error);
    }
  }

  save(): void {
    if (this.isValidForm(this.formInvoice)) {

      if (this.invoiceDetails().length == 0) {
        this.showMessageError('No hay Productos Seleccionados');

      } else {
        const typeCtrl = this.formInvoice.controls['type'].value;
        const ivaCtrl = this.formInvoice.controls['ivaCondition'].value;

        const model: InvoiceModel = {
          id: 0,
          customerId: this.customerId,
          userId: this.userId(),
          invoiceNumber: 0,
          cae: null,
          customerName: this.formInvoice.controls['customerName'].value,
          customerCuit: this.formInvoice.controls['customerCuit'].value,
          customerAddress: this.formInvoice.controls['customerAddress'].value,
          customerEmail: this.formInvoice.controls['customerEmail'].value,
          observation: this.formInvoice.controls['observation'].value,
          dateTime: this.formInvoice.controls['dateTime'].value,
          iva21: 0,
          iva27: 0,
          iva10: 0,
          total: this.total(),
          ivaTotal: this.ivaTotal(),
          ivaSelected: this.ivaSelected,
          caeExpirationTime: null,
          integrationSuccess: false,
          type: (typeCtrl === eInvoiceType.A)
            ? (ivaCtrl === eIvaCondition.RespMonotributo
              ? eInvoiceType.RespMonotributo
              : eInvoiceType.A)
            : (ivaCtrl === eIvaCondition.Exento
              ? eInvoiceType.EXENTO
              : eInvoiceType.B),
          invoiceDetails: this.invoiceDetails(),
          version: InvoiceVersion.Arca
        };
        this.isSaving.set(true);
        this.serviceInvoice.saveInvoice(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Comprobante creado correctamente`
              );
              this.isSaving.set(false);
              this.popComponent.isConfirmationvisible.set(false);
              this.router.navigate(['/home/invoices']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error.descripcion)
            }
          });

      }

    }
  };

  startEdit(id: number): void {
    this.editId.set(id);
  };

  startEditIva(id: number): void {
    this.editIdIva.set(id);
  }

  startEditProductName(id: number): void {
    this.editIdProductName.set(id);
  }

  startEditProductPrice(id: number): void {
    this.editIdProductPrice.set(id);
  }
  stopEdit(): void {
    this.editId.set(null);
  }

  stopEditIva(): void {
    this.editIdIva.set(null);
  }
  
  stopEditProductName(): void {
    this.editIdProductName.set(null);
  }

  stopEditProductPrice(): void {
    this.editIdProductPrice.set(null);
  }

  changeQuantity(quantity: number): void {

    if (!quantity || quantity <= 0) quantity = 1;

    this.invoiceDetailsList.update(list =>
      list.map(detail =>
        detail.ownCode === this.editId()
          ? { ...detail, quantity, subTotal: quantity * detail.price }
          : detail
      )
    );

    this.invoiceDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editId()
          ? { ...detail, quantity }
          : detail
      )
    );

  };

  changeIvaValue(iva: number, id: number): void {
    let newIva = Number(iva);
    try {
      this.invoiceDetailsList.update(list =>
        list.map(detail =>
          detail.productId === id
            ? { ...detail, iva: newIva }
            : detail
        )
      );

      this.invoiceDetails.update(list =>
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
  };

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
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
    const model: InvoiceDetailList = invoiceGridParser(newEditProduct, this.iva, this.bindPrice(newEditProduct));

    this.invoiceDetailsList.update(item => [...item, model]);
    /* Parseo dato a Dto Factura Detalle */
    const modelDetail: InvoiceDetails = invoiceDetailParser(newEditProduct, this.iva, this.bindPrice(newEditProduct));
    this.invoiceDetails.update(prod => [...prod, modelDetail]);

    this.formProductSearch.controls['productSearchFilter'].setValue('');
  }

  changeProductName(name: string): void {
    this.invoiceDetailsList.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );

    this.invoiceDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );
  }

  changeProductPrice(price: number): void {
    this.invoiceDetailsList.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductPrice()
          ? { ...detail, price, subTotal: detail.quantity * price }
          : detail
      )
    );

    this.invoiceDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductPrice()
          ? { ...detail, price }
          : detail
      )
    );

  }

  isValidProductName(): boolean {
    return this.invoiceDetails().every(y => typeof y.productName === "string" && y.productName.trim() !== "")
  }

  isValidPayment(): boolean {
    let result = this.formInvoice.controls['payment'].invalid;

    if (result) {
      this.formInvoice.controls['payment'].markAsDirty();
      this.formInvoice.controls['payment'].updateValueAndValidity();
    }

    return !result;
  }

  updatePriceByPaymentSelectedChange(): void {
    if (this.invoiceDetails().length > 0) {

      this.isLoading.set(true);
      let invoiceDetailsAux = this.invoiceDetailsList().filter(data => data.productId > 0);

      if (invoiceDetailsAux.length > 0) {

        this.invoiceDetailsList.set([]);
        this.invoiceDetails.set([]);

        const observables = invoiceDetailsAux.map(data =>
          this.serviceProduct.getById(data.productId).pipe(
            map(product => ({
              product,
              iva: data.iva,
              quantity: data.quantity
            }))
          )
        );

        forkJoin(observables).subscribe({
          next: (results) => {
            results.forEach(({ product, iva, quantity }) => {
              const model: InvoiceDetailList = invoiceGridParser(product, iva, this.bindPrice(product), quantity);
              this.invoiceDetailsList.update(prod => [...prod, model]);

              const modelDetail: InvoiceDetails = invoiceDetailParser(product, iva, this.bindPrice(product), quantity);
              this.invoiceDetails.update(prod => [...prod, modelDetail]);
            });

            this.isLoading.set(false);
          },
          error: (err) => {
            console.error('Error al obtener productos', err);
            this.isLoading.set(false);
          }
        });
      }
      this.isLoading.set(false);
    };
  }

  onChange(customer: CustomerModel) {
    if (typeof customer == "string") {
      this.customerId = 0;
      return
    }
    if (customer != null) {
      this.formInvoice.patchValue({
        customerAddress: customer.address,
        customerCuit: this.clearCuitString(customer.cuit),
        customerName: customer.name,
        customerEmail: customer.email
      }, { emitEvent: false });
      this.customerId = customer.id;
    }
  }

  clearCuitString(data: string): string {
    return data.replace(/\D/g, '');
  }

  updatePattern(): void {
    const type = this.formInvoice.get('type')?.value;
    const ivaCondition = this.formInvoice.get('ivaCondition')?.value;
    const cuitControl = this.formInvoice.get('customerCuit');

    cuitControl?.clearValidators();

    if (type === 2 && ivaCondition === eIvaCondition.ConsFinal) {
      cuitControl?.setValidators([Validators.required, Validators.pattern(/^\d{8}$/)]);
    } else {
      cuitControl?.setValidators([Validators.required, Validators.pattern(/^\d{11}$/)]);
    }

    cuitControl?.updateValueAndValidity({ emitEvent: false });
  }

  getInvoice(id: number): void {
    if (id != 0) {
      this.isInvoiceEditable.set(false);
      this.id.set(id);
      this.serviceInvoice.getInvoiceById(id).subscribe({
        next: (r: any) => {
          this.formInvoice.patchValue({
            dateTime: r.dateTime,
            type: this.mapInvoiceType(r.type),
            customerAddress: r.customerAddress,
            customerCuit: r.customerCuit,
            customerName: r.customerName,
            customerEmail: r.customerEmail,
            observation: r.observation,
            ivaCondition: this.mapCondIva(r.type)
          });
          this.version.set(r.version);
          //   this.invoiceNumber = r.invoiceNumber,
          //   this.cae = r.cae,
          //   this.iva21 = r.iva21,
          //   this.iva27 = r.iva27,
          //   this.iva10 = r.iva10,
          this.invoiceDetailsList.set(r.invoiceDetails.map((element: InvoiceDetails) => {
            return invoiceDetailToGridParser(element)
          }));
          this.isLoading.set(false);
        },
        error: () => { this.isLoading.set(false); }
      });
    }
    this.getIntegrationLog(id);
  }

  mapCondIva(type: eInvoiceType): number {
    const map: Record<eInvoiceType, eIvaCondition> = {
      [eInvoiceType.A]: eIvaCondition.RespInscrip,
      [eInvoiceType.B]: eIvaCondition.ConsFinal,
      [eInvoiceType.C]: eIvaCondition.ConsFinal,
      [eInvoiceType.EXENTO]: eIvaCondition.Exento,
      [eInvoiceType.RespMonotributo]: eIvaCondition.RespMonotributo
    };

    return map[type] ?? 0;
  }

  mapInvoiceType(type: eInvoiceType): number {
    switch (type) {
      case eInvoiceType.A:
      case eInvoiceType.RespMonotributo:
        return 1;

      case eInvoiceType.B:
      case eInvoiceType.C:
      case eInvoiceType.EXENTO:
        return 2;

      default:
        return 0;
    }
  }

  toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
      this.formInvoice.enable();
      this.formProductSearch.enable();
      Object.keys(this.formInvoice.controls).forEach(key => {
        const control = this.formInvoice.get(key);
        if (control) {
          control.updateValueAndValidity({ emitEvent: false });
        }
      });

    } else {
      this.formInvoice.disable();
      this.formProductSearch.disable();
    }
  }

  searchProduct(): void {
    if (!this.isValidPayment()) return;

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

    this.isLoading.set(true);

    this.serviceProduct.getProducts(productParams).subscribe({
      next: (r) => {
        if (r.data.length === 1) {
          const product: ProductsModel = r.data[0];
          this.addOrUpdateProduct(product);
        }
        else {
          this.isLoading.set(false);
          this.openComponentProduct();
        }

        this.isLoading.set(false);
        this.formProductSearch.reset();
      },
      error: () => {
        this.isLoading.set(false);
        this.formProductSearch.reset();
      }
    });
  }

  addOrUpdateProduct(product: ProductsModel): void {
    const existingDetail = this.invoiceDetails().find(item => item.productId === product.id);

    if (existingDetail) {
      // actualizar cantidad y subtotal
      existingDetail.quantity += 1;

      const gridDetail = this.invoiceDetailsList().find(item => item.ownCode === product.id);
      if (gridDetail) {
        gridDetail.quantity += 1;
        gridDetail.subTotal = gridDetail.quantity * this.bindPrice(product);
      }
    } else {
      // agregar nuevo producto
      const gridDetail: InvoiceDetailList = invoiceGridParser(product, this.iva, this.bindPrice(product));
      const detail: InvoiceDetails = invoiceDetailParser(product, this.iva, this.bindPrice(product));

      this.invoiceDetailsList.update(list => [...list, gridDetail]);
      this.invoiceDetails.update(list => [...list, detail]);
    }

  }

  openComponentProduct(): void {
    if (true) {
      const drawerRefProduct = this.drawerService.create<
        InvoiceProductSearchComponent,
        { filter: string; supplierId: number },
        [ProductsModel]
      >({
        nzTitle: 'Productos',
        nzContent: InvoiceProductSearchComponent,
        nzSize: 'large',
        nzWidth: '90%',
        nzContentParams: {
          filter: this.formProductSearch.controls['productSearchFilter'].value,
        },
        nzClosable: false,
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
          this.invoiceDetailsList.set([]);
        },
      });
    } else {
      return;
    }
  }

  getIntegrationLog(id: number):Array<InvoiceLog> | any {
    this.logLoading.set(true);
    this.serviceInvoice.getIntegrationLogById(id).subscribe({
      next:(r: Array<InvoiceLog>) =>{
        this.invoiceLog.set(r);
        this.logLoading.set(false);
      },
      error:(e) =>{
        this.logLoading.set(false);
      }
    })
  }

  parserXML(data: string){    
    const xmlParser = new XMLParser();
    if (data == null) {return '';}
    let parsed = xmlParser.parse(data);
    return JSON.stringify(parsed, null, 2)
  }

}