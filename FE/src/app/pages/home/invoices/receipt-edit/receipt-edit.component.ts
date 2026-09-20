import {
  OnInit,
  Component,
  TemplateRef,
  ViewChild,
  ElementRef,
  Inject,
  LOCALE_ID,
  signal,
  computed,
} from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { formatCurrency } from '@angular/common';
import { NzDrawerModule, NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { differenceInCalendarDays } from 'date-fns';
import { InvoiceType } from '../model/invoice-type.Enum';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../products/product.service';
import { InvoiceService } from '../invoices.service';
import { EntityService } from '../../customers/customer.service';
import {
  receiptDetailParser,
  receiptDetails,
  receiptDetailsGrid,
  receiptDetailToGridParser,
  receiptGridParser,
  receiptModel,
} from '../model/receipt.model';
import { CustomerAddModel } from '../../customers/model/customer.add.model';
import { ProductsModel } from '../../products/model/product.model';
import { InvoiceProductSearchComponent } from '../invoice-product-search/invoice-product-search.component';
import { IvaType } from '../model/iva-type.Enum';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { HeaderOperationsButtonsComponent } from '../../../../common/components/headers/buttons.oparations.header.component';
import { AuthService } from '../../../../common/auth/interceptors/auth.service';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';

@Component({
  selector: 'app-receipt-edit',
  templateUrl: './receipt-edit.component.html',
  styleUrls: ['./receipt-edit.component.css'],
  imports: [NzPageHeaderModule, FormsModule, ReactiveFormsModule, NzCollapseModule, NzLayoutModule, NzInputModule, NzFormModule, NzDatePickerModule, NzSelectModule, NzDividerModule, NzSwitchModule, NzTableModule, NzInputNumberModule, NzSpaceModule, NzIconModule, PopupConfirmationComponent, NzDrawerModule, NzButtonModule, HeaderOperationsButtonsComponent, FormatDatePipe]
})
export class ReceiptEditComponent extends BaseComponent implements OnInit {

  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  @ViewChild('drawerTemplate', { static: false }) drawerTemplate?: TemplateRef<{
    $implicit: { filter: string };
    drawerRef: NzDrawerRef<string>;
  }>;

  loading = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(true);
  permissions = Permission;
  id = signal<number>(0);
  startDate = Date.now();
  type = InvoiceType;
  ivaType = IvaType;
  typeSelectedId: number = 1;
  ivaSelectedId: number = 1;
  ivaSelected!: number;
  iva10: number = parseFloat('10.5');
  iva21: number = 21;
  iva27: number = 27;
  invoiceA: boolean = true;
  cuit!: string;
  supplierId!: number;
  iva: number = 21;

  percIngBrutos = signal(0);
  percIva = signal(0);
  concNoGravado = signal(0);

  subtotal = computed(() =>
    this.receiptDetailList().reduce((acc, item) =>
      acc + (item.quantity * this.ivaCalculate(item.price, item.iva)), 0)
  );

  total = computed(() =>
    this.receiptDetailList().reduce((acc, item) =>
      acc + item.price * item.quantity, this.concNoGravado() + this.percIngBrutos() + this.percIva())
  );

  ivaTotal = computed(() =>
    this.receiptDetailList().reduce((acc, item) =>
      acc + (item.price - this.ivaCalculate(item.price, item.iva)) * item.quantity, 0)
  );

  product= signal<string>('');
  editProductId: number = 0;

  today = new Date();

  formReceipt!: FormGroup;
  formProductSearch!: FormGroup;
  formProduct!: FormGroup;
  formSupplierSearch!: FormGroup;
  formReceiptModel!: FormGroup;

  receiptDetailList = signal<receiptDetailsGrid[]>([]);
  receiptDetails = signal<receiptDetails[]>([]);
  /*
   **Variables de la tabla detalle
   */
  editId = signal<number | null>(null);
   editIdIva = signal<number | null>(null);
  value!: string;
  value1!: string;
  value2!: string;
  value3!: string;

  userId = signal<number>(0);
  selectedDni= signal<boolean>(false);
  dni: any;
  editIdProductName = signal<number | null>(null);
  editIdProductPrice = signal<number | null>(null);
  /*
   ** Parametros de busqueda
   */
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 10,
  };

  constructor(
    private fb: FormBuilder,
    notificacionService: NzNotificationService,
    private serviceEntity: EntityService,
    private serviceProduct: ProductService,
    private serviceInvoice: InvoiceService,
    public serviceUser: AuthService,
    el: ElementRef,
    private router: Router,
    message: NzMessageService,
    private drawerService: NzDrawerService,
    @Inject(LOCALE_ID) public locale: string,
    private route: ActivatedRoute,
  ) {
    super(notificacionService, el, message);
    this.formReceipt = this.fb.group({
      dateTime: [{ value: new Date(this.startDate), disabled: true }, Validators.required],
      type: [{ value: 1, disabled: true }, Validators.required],
      receiptNumber: [{ value: '', disabled: true }, Validators.required],
      supplierAddress: [{ value: '', disabled: true }, Validators.required],
      supplierCuit: [{ value: '', disabled: true }, Validators.required, Validators.pattern(/^[0-9]{11}$/)],
      supplierDni: [{ value: '', disabled: true }],
      supplierName: [{ value: '', disabled: true }, Validators.required],
      observation: [{ value: '', disabled: true }],
    });
    this.formSupplierSearch = this.fb.group({});
    this.formProductSearch = this.fb.group({
      productSearchFilter: [{ value: '', disabled: true }],
    });
  }

  formatter = (data: number = 0) =>
    formatCurrency(data, this.locale, '$', 'ARS', '1.1-2');

  ngOnInit(): void {
    this.userId.set(this.serviceUser.currentUser()?.id ?? 0);
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.isLoading.set(true);
          this.getInvoice(p['id']);
        }
      },
      error: () => { }
    });
  }

  typeSelectedChange(id: any): void {
    this.typeSelectedId = id;
    if (id == 1) {
      this.invoiceA = true;
    } else {
      this.invoiceA = false;
    }
  }

  startEdit(id: number): void {
    this.editId.set(id);
  }

  startEditIva(id: number): void {
    this.editIdIva.set(id);
  }

  stopEdit(): void {
    this.editId.set(null);
  }

  stopEditIva(): void {
    this.editIdIva.set(null);
  }

  changeIvaValue(iva: number, id: number): void {
    let newIva = Number(iva);
    try {
      this.receiptDetailList.update(list =>
        list.map(detail =>
          detail.productId === id
            ? { ...detail, iva: newIva }
            : detail
        )
      );

      this.receiptDetails.update(list =>
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

  concNoGravadoChange(value: number): any {
    this.concNoGravado.set(value);
  }

  percIvaChange(value: number): any {
    this.percIva.set(value);
  }

  percIngBrutosChange(value: number): any {
    this.percIngBrutos.set(value);
  }

  disabledDate = (current: Date): boolean =>
    // Can not select days before today and today
    differenceInCalendarDays(current, this.today) > 0;

  searchSupplier(): void {
    this.cuit = this.formReceipt.controls['supplierCuit'].value;
    if (this.cuit === '00') {
      this.formReceipt.controls['supplierAddress'].setValue('-');
      this.formReceipt.controls['supplierCuit'].setValue('99999999995');
      this.formReceipt.controls['supplierName'].setValue('-');
      this.supplierId = 0;
      return;
    } else {
      if (this.cuit.length >= 6) {
        this.serviceEntity.getSupplierByCuit(this.cuit).subscribe({
          next: (data: any) => {
            this.formReceipt.controls['supplierAddress'].setValue(data.address);
            this.formReceipt.controls['supplierCuit'].setValue(data.cuit);
            this.formReceipt.controls['supplierName'].setValue(data.name);
            this.supplierId = data?.id;
          },
          error: () => {
            this.showMessageError('No se encontro Proveedor');
          },
        });
      }
    }
  }

  // openComponentSupplier(): void {
  //   const drawerRefSupplier = this.drawerService.create<
  //     ReceiptSupplierSearchComponent,
  //     {},
  //     CustomerAddModel
  //   >({
  //     nzTitle: 'Proveedor',
  //     nzContent: ReceiptSupplierSearchComponent,
  //     nzSize: 'large',
  //     nzWidth: '90%',
  //     nzClosable: false,
  //   });
  //   drawerRefSupplier.afterClose.subscribe({
  //     next: (data) => {
  //       if (data != undefined) {
  //         this.supplierId = data.id;
  //         this.formReceipt.controls['supplierAddress'].setValue(data.address);
  //         this.formReceipt.controls['supplierCuit'].setValue(!isNil(data.cuit) ? data.cuit.replace(/[^a-zA-Z0-9 ]/g, '') : null);
  //         this.formReceipt.controls['supplierName'].setValue(data.name);
  //         this.formReceipt.controls['supplierDni'].setValue(data.dni)
  //       }
  //     },
  //     error: () => { },
  //   });
  // }

  changeQuantity(quantity: number): void {
    if (!quantity || quantity <= 0) quantity = 1;

    this.receiptDetailList.update(list =>
      list.map(detail =>
        detail.ownCode === this.editId()
          ? { ...detail, quantity, subTotal: quantity * detail.price }
          : detail
      )
    );

    this.receiptDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editId()
          ? { ...detail, quantity }
          : detail
      )
    );
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2');
  }

  ivaCalculate(data: number, iva: number): number {
    let newIva = 1 + (iva / 100);
    return (data / newIva);
  }

  save(): void {
    if (this.isValidForm(this.formReceipt)) {
      if (this.selectedDni() && this.formReceipt.controls['supplierDni'].value.length < 8) {
        return this.showMessageError('DNI Invalido');
      };
      if (this.receiptDetails().length == 0) {
        this.showMessageError('No hay Productos Seleccionados');
      } else {
        const model: receiptModel = {
          id: 0,
          supplierId: this.supplierId,
          userId: this.userId(),
          receiptNumber: this.formReceipt.controls['receiptNumber'].value,
          supplierName: this.formReceipt.controls['supplierName'].value,
          supplierCuit: this.formReceipt.controls['supplierCuit'].value,
          supplierAddress: this.formReceipt.controls['supplierAddress'].value,
          observation: this.formReceipt.controls['observation'].value,
          dateTime: this.formReceipt.controls['dateTime'].value,
          total: this.total(),
          ivaTotal: this.ivaTotal(),
          type: this.formReceipt.controls['type'].value,
          concNoGravado: this.concNoGravado(),
          percIva: this.percIva(),
          percIngBrutos: this.percIngBrutos(),
          receiptDetails: this.receiptDetails(),
        };
        this.isSaving.set(true)
        this.serviceInvoice.saveReceipt(model).subscribe({
          next: () => {
            this.showNotificationSuccess(
              'Guardado correcto',
              `Comprobante creado correctamente`
            );
            this.isSaving.set(false);
            this.router.navigate(['/home/receipts']);
          },
          error: () => {
            this.isSaving.set(false);
            this.showMessageError('No se pudo crear el Comprobante');
          },
        });
      }
    }
  }

  handleOk() {
    try {
      const idToDelete = this.popupComponent.elementSelectedToDelete();

      const updatedList = this.receiptDetailList().filter(el => el.ownCode !== idToDelete);
      const updatedDetails = this.receiptDetails().filter(el => el.productId !== idToDelete);

      this.receiptDetailList.set(updatedList);
      this.receiptDetails.set(updatedDetails);

      this.popupComponent.isDeleteConfirmationVisible.set(false);

    } catch (error) {
      console.log(error);
    }
  }

  msjConfirmOk() {
    try {
      this.receiptDetailList.set(this.receiptDetailList().filter(
        (element) => element.productId != this.popupComponent.elementSelected()));

      this.popupComponent.isConfirmationvisible.set(false);

      if (this.receiptDetailList().length == 0) {
        this.showMessageError('No ha seleccionado producto');
        return;
      }

      if (this.isValidForm(this.formReceipt) && this.isValidForm(this.formSupplierSearch) && this.isValidForm(this.formProductSearch)) {
        this.popComponent.showConfirmation()
      }
    } catch (error) { }
  }

  searchProduct(): void {

    if (this.isValidForm(this.formReceipt)) { return; }

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

  openComponentProduct(): void {
    if (this.formProductSearch.controls['productSearchFilter'].value == '00') {
      this.addNewEditProduct();
      return;
    }
    const drawerRefProduct = this.drawerService.create<
      InvoiceProductSearchComponent,
      { filter: string, supplierId: number | null },
      [ProductsModel]
    >({
      nzTitle: 'Productos',
      nzContent: InvoiceProductSearchComponent,
      nzSize: 'large',
      nzWidth: '90%',
      nzContentParams: {
        filter: this.formProductSearch.controls['productSearchFilter'].value,
        supplierId: this.supplierId
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
        this.receiptDetailList.set([]);
        this.formProductSearch.controls['productSearchFilter'].setValue('');
      },
    });

  }

  select() {
    this.selectedDni.set(!this.selectedDni());
    if (this.selectedDni()) {
      this.dni = this.formReceipt.controls['supplierDni'].value
    } else {
      this.dni = null;
    }
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
    const model: receiptDetailsGrid = receiptGridParser(newEditProduct, this.iva);
    this.receiptDetailList.update(item => [...item, model]);

    /* Parseo dato a Dto Factura Detalle */
    const modelDetail: receiptDetails = receiptDetailParser(newEditProduct, this.iva);
    this.receiptDetails.update(prod => [...prod, modelDetail]);

    this.loading.set(false);
    this.formProductSearch.controls['productSearchFilter'].setValue(
      ''
    );
  }

  startEditProductName(id: number): void {
    this.editIdProductName.set(id);
  }

  changeProductName(name: string): void {
    this.receiptDetailList.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );

    this.receiptDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );

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

  changeProductPrice(price: number): void {
    this.receiptDetailList.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductPrice()
          ? { ...detail, price, subTotal: detail.quantity * price }
          : detail
      )
    );

    this.receiptDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductPrice()
          ? { ...detail, price }
          : detail
      )
    );
  }

  addOrUpdateProduct(product: ProductsModel): void {
    const existingDetail = this.receiptDetails().find(item => item.productId === product.id);

    if (existingDetail) {
      // actualizar cantidad y subtotal
      existingDetail.quantity += 1;

      const gridDetail = this.receiptDetailList().find(item => item.ownCode === product.id);
      if (gridDetail) {
        gridDetail.quantity += 1;
        gridDetail.subTotal = gridDetail.quantity * product.purchasePrice;
      }
    } else {
      // agregar nuevo producto
      const gridDetail: receiptDetailsGrid = receiptGridParser(product, this.iva);
      const detail: receiptDetails = receiptDetailParser(product, this.iva);

      this.receiptDetailList.update(list => [...list, gridDetail]);
      this.receiptDetails.update(list => [...list, detail]);
    }

  }

  getInvoice(id: number): void {
    if (id != 0) {
      this.isInvoiceEditable.set(false);
      this.id.set(id);
      this.serviceInvoice.getReceiptById(id).subscribe({
        next: (r) => {
          this.formReceipt.patchValue({
            dateTime: r.dateTime,
            type: r.type,
            receiptNumber: r.receiptNumber,
            supplierAddress: r.supplierAddress,
            supplierCuit: r.supplierCuit,
            supplierName: r.supplierName,
            observation: r.observation,
          });
          this.userId.set(r.userId),
            this.concNoGravado.set(r.concNoGravado),
            this.percIngBrutos.set(r.percIngBrutos),
            this.percIva.set(r.percIva),
            this.receiptDetailList.set(r.receiptDetails.map((element : receiptDetails) =>{
            return receiptDetailToGridParser(element)} ));
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        },
      });
    }
  }

  toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
      this.formReceipt.enable();
      this.formProductSearch.enable();
    } else {
      this.formReceipt.disable();
      this.formProductSearch.disable();
    }
  }
}

