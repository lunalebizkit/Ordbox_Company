import { Component, computed, ElementRef, Inject, LOCALE_ID, OnInit, signal, TemplateRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { BudgetsService } from '../budgets.services';
import { BudgetGrid, BudgetDetailParser, BudgetDetails, BudgetGridParser, BudgetsModel, budgetsDetailFromParser, budgetsGridFromParser } from '../model/budgets.model';
import { formatCurrency, formatDate } from '@angular/common';
import { differenceInCalendarDays } from 'date-fns';
import { BudgetType } from "../model/budgets-type.Enum";
import { ProductService } from "../../products/product.service";
import { ProductsModel } from "../../products/model/product.model";
import { NzDrawerModule, NzDrawerService } from "ng-zorro-antd/drawer";
import { InvoiceProductSearchComponent } from "../../invoices/invoice-product-search/invoice-product-search.component";
import { ePayment } from "../../invoices/model/invoice-payment.Enum";
import { forkJoin, map } from "rxjs";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
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
import { FormatDatePipe } from "../../../../common/pipes/date.pipe";
import { eInvoiceType } from "../../invoices/model/invoice-type.Enum";
import { Permission } from "../../../../common/auth/models/permissions.enum";

@Component({
  selector: 'app-budgets-edit',
  templateUrl: './budgets-edit.component.html',
  styleUrls: ['./budgets-edit.component.css'],
  imports: [HeaderOperationsButtonsComponent, FormsModule, NzFormModule, ReactiveFormsModule, NzCollapseModule, NzLayoutModule, FormatDatePipe, NzTagModule, NzPageHeaderModule, NzSelectModule, NzDividerModule, NzSwitchModule, AppCommonModule, NzTableModule, PopupConfirmationComponent, NzInputNumberModule, NzInputModule, NzSpaceModule, NzDrawerModule]
})

export class BudgetsEditComponent extends BaseComponent implements OnInit {

  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  loading = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);

  type = BudgetType;
  typeSelectedId: number = 1;
  form!: FormGroup;
  id = signal<number>(0);
  userId = signal<number>(0);
  paymentSelected: any;
  /*FORM*/
  formProductSearch!: FormGroup;
  startDate = new Date;
  today = new Date();

  product= signal<string>('');
  cuit!: string;
  payment: { value: string; label: string }[] = Object.entries(ePayment).map(([value, label]) => ({ value, label }))
  /*Lista de productos */
  permissions = Permission;
  editProductId: number = 0;
  editId = signal<number | null>(null);
  editIdIva = signal<number | null>(null);
  editIdProductPrice = signal<number | null>(null);
  editIdProductName = signal<number | null>(null);
  dataDetails = signal<BudgetDetails[]>([]);
  dataGrid = signal<BudgetGrid[]>([]);
  cae = signal<string | null>(null);
  integrationSuccess = signal<boolean | null>(null);
  customerId = signal<number>(0);
  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(false);

  subtotal = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.quantity * item.price, 0)
  );

  total = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.price * item.quantity, 0)
  );

  /*
  ** Parametros de busqueda
  */
  queryParams = {
    filter: {
      product: '',
      brand: 0,
      category: 0,
      status: 0,
      supplier: []
    },
    page: 0,
    pageSize: 50
  };

  selectedDni: boolean = false;
  dni: any;

  constructor(
    private service: BudgetsService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private router: Router,
    private serviceProduct: ProductService,
    public serviceUser: AuthService,
    private drawerService: NzDrawerService,
    private serviceBudget: BudgetsService,
    @Inject(LOCALE_ID) public locale: string


  ) {
    super(notificacionService, el, message);
    this.form = this.fb.group({
      id: [0, Validators.required],
      budgetNumber: [{ value: 0, disabled: true }, Validators.required],
      customerName: [{ value: '', disabled: true }, Validators.required],
      payment: [{ value: '', disabled: true }, Validators.required],
      customerAddress: [{ value: '', disabled: true }, Validators.required],
      dateTime: [new Date(this.startDate), Validators.required],
      observation: [{ value: '', disabled: true }]
    });

    this.formProductSearch = this.fb.group({
      productSearchFilter: [{ value: '', disabled: true }]
    })

  }

  ngOnInit(): void {
    this.userId.set(this.serviceUser.currentUser.id);
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.getBudget(p['id']);
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

  getBudget(id: number): void {
    if (id != 0) {
      this.isLoading.set(true);
      this.id.set(id);
      this.service.getById(id).subscribe({
        next: (r) => {
          this.form.patchValue({
            id: r.id,
            budgetNumber: r.budgetNumber,
            dateTime: r.dateTime,
            payment: r.payment,
            customerAddress: r.customerAddress,
            customerName: r.customerName,
            observation: r.observation,
            type: this.mapInvoiceType(r.type),
            customerCuit: r.customerCuit,
            customerDni: '',
            invoiceNumber: r.invoiceNumber,
          });
          this.paymentSelectedChange(r.payment);
          this.startDate = new Date(r.dateTime.toString());

          this.dataGrid.set(r.budgetDetails.map((modelDetail: BudgetGrid, index: number) => {
            return budgetsGridFromParser(modelDetail, index)
          }));
          
          this.dataDetails.set(r.budgetDetails.map((modelDetail: BudgetGrid, index: number) => {
            return budgetsDetailFromParser(modelDetail, index)
          }));

          this.form.controls['payment'].disable();
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      })
    }
  }

  paymentSelectedChange(id: any): void {
    this.paymentSelected = id;
    this.updatePriceByPaymentSelectedChange();
  }


  save(): void {
    if (this.isValidForm(this.form)) {
      //EDITAR
      if (this.id() > 0) {
        const model: BudgetsModel = {
          id: this.id(),
          customerName: this.form.controls['customerName'].value,
          payment: this.form.controls['payment'].value,
          budgetNumber: this.form.controls['budgetNumber'].value,
          customerAddress: this.form.controls['customerAddress'].value,
          observation: this.form.controls['observation'].value,
          userId: this.userId(),
          total: 0,
          dateTime: this.form.controls['dateTime'].value,
          budgetDetails: this.dataDetails()
        };

        this.isSaving.set(true);
        this.serviceBudget.editBudget(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Presupuesto editado correctamente`
              );
              this.isSaving.set(false);
              this.router.navigate(['/home/budgets']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error)
            }
          });

        //GUARDAR
      } else {
        const model: BudgetsModel = {
          id: 0,
          customerName: this.form.controls['customerName'].value,
          payment: this.form.controls['payment'].value,
          budgetNumber: this.form.controls['budgetNumber'].value,
          customerAddress: this.form.controls['customerAddress'].value,
          observation: this.form.controls['observation'].value,
          userId: this.userId(),
          total: 0,
          dateTime: this.form.controls['dateTime'].value,
          budgetDetails: this.dataDetails(),
        };


        this.isSaving.set(true);
        this.service.saveBudget(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Presupuesto creado correctamente`
              );
              this.isSaving.set(false);
              this.router.navigate(['/home/budgets']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error)
            }
          });
      }
    }

  };

  startEdit(id: number): void {
    this.editId.set(id);
  }

  stopEdit(): void {
    this.editId.set(null);
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

  typeSelectedChange(id: any): void {
    this.typeSelectedId = id;
  }

  disabledDate = (current: Date): boolean =>

    differenceInCalendarDays(current, this.today) > 0;


  msjConfirmOk() {
    try {
      this.dataGrid.set(this.dataGrid().filter(element => element.ownCode != this.popupComponent.elementSelected()));

      this.popupComponent.isConfirmationvisible.set(false);

      if (this.dataGrid().length === 0) {
        this.showMessageError('No ha seleccionado producto');
        return;
      }

      if (this.isValidForm(this.form) &&
        this.isValidForm(this.formProductSearch)) {
        this.popComponent.showConfirmation()
      }
    } catch (error) { console.log(error); }
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
  }

  searchCustomer(): void {

    this.cuit =
      this.form.controls['customerCuit'].value;
    if (this.cuit == '00') {
      this.form.controls['customerAddress'].setValue('S/D');
      this.form.controls['customerCuit'].setValue('99999999995');
      this.form.controls['customerName'].setValue('-');
      this.customerId.set(this.serviceUser.currentUser.id);
      return;
    } else {
      if (this.cuit.length >= 6) {
        this.serviceBudget.getByCuit(this.cuit).subscribe({
          next: (data) => {
            this.form.controls['customerAddress'].setValue(data.address);

            this.form.controls['customerName'].setValue(data.name);

          },
          error: () => { this.showMessageError('No se encontro Cliente'); }
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
  };

  openComponentProduct(): void {
    const drawerRefProduct = this.drawerService.create<InvoiceProductSearchComponent, { filter: string }, [ProductsModel]>({
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
    })
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
      const gridDetail: BudgetGrid = BudgetGridParser(product, this.bindPrice(product));
      const detail: BudgetDetails = BudgetDetailParser(product, this.bindPrice(product));

      this.dataGrid.update(list => [...list, gridDetail]);
      this.dataDetails.update(list => [...list, detail]);
    }
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
  }

  addNewEditProduct(): void {
    this.editProductId--;
    let product: ProductsModel = {
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
    const model: BudgetGrid = BudgetGridParser(product, this.bindPrice(product));
    this.dataGrid.update(item => [...item, model]);

    /* Parseo dato a Dto Factura Detalle */
    const modelDetail: BudgetDetails = BudgetDetailParser(product, this.bindPrice(product));
    this.dataDetails.update(item => [...item, modelDetail]);

    this.isLoading.set(false);

    this.formProductSearch.controls['productSearchFilter'].setValue('');
  }

  stopEditProductName(): void {
    this.editIdProductName.set(null);
  }

  startEditProductName(id: number): void {
    this.editIdProductName.set(id);
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

  updatePriceByPaymentSelectedChange(): void {
    if (this.dataDetails().length === 0) return;

    this.isLoading.set(true);

    const invoiceDetailsAux = this.dataDetails().filter(d => d.productId > 0);

    if (invoiceDetailsAux.length === 0) {
      this.isLoading.set(false);
      return;
    }

    const observables = invoiceDetailsAux.map(data =>
      this.serviceProduct.getById(data.productId).pipe(
        map(product => ({
          product,
          quantity: data.quantity
        }))
      )
    );

    forkJoin(observables).subscribe({
      next: (results) => {
        this.dataGrid.set(
          results.map(({ product, quantity }) =>
            BudgetGridParser(product, this.bindPrice(product), quantity)
          ));

        this.dataDetails.set(
          results.map(({ product, quantity }) =>
            BudgetDetailParser(product, this.bindPrice(product), quantity)
          ));

        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error al obtener productos', err);
        this.isLoading.set(false);
      }
    });
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
